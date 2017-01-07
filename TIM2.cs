using System;
using System.IO;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace IMGZ_Editor
{
    class TIM2 : ImageContainer
    {
        public static readonly IList<string> extensions = new List<string> { "tm2" }.AsReadOnly();
        
        private enum CI_TYPE : byte
        {
            None = 0,
            RGB16 = 1,
            RGB24 = 2,
            RGB32 = 3,
            INDEX4 = 4,
            INDEX8 = 5
        }
        private static CI_TYPE BaseType(CI_TYPE value) { return (CI_TYPE)((byte)value & 0x3F); }
        private static void InvertChannels(byte[] buffer, CI_TYPE imageType, bool fromTIM)
        {
            byte cache;
            switch (imageType)
            {
                case CI_TYPE.RGB16:
                    //Swap Red and Blue
                    for (int j = 0; j < buffer.Length; j += 2)
                    {
                        cache = (byte)(buffer[j] & 0x1F);
                        buffer[j] = (byte)((buffer[j] & 0xE0) | ((buffer[j + 1] & 0x7C) >> 2));
                        buffer[j + 1] = (byte)((buffer[j + 1] & 0x83) | (cache << 2));
                    }
                    break;
                case CI_TYPE.RGB24:
                    //Swap Red and Blue
                    for (int j = 0; j < buffer.Length; j += 3)
                    {
                        cache = buffer[j];
                        buffer[j] = buffer[j + 2];
                        buffer[j + 2] = cache;
                    }
                    break;
                case CI_TYPE.RGB32:
                    //Swap Red and Blue
                    //Double alpha
                    for (int j = 0; j < buffer.Length; j += 4)
                    {
                        cache = buffer[j];
                        buffer[j] = buffer[j + 2];
                        buffer[j + 2] = cache;
                        if (fromTIM) { buffer[j + 3] = (byte)(Math.Min(buffer[j + 3] * 2, 255)); }
                        else { buffer[j + 3] = (byte)Math.Ceiling(buffer[j + 3] / 2.0); }
                    }
                    break;
                case CI_TYPE.INDEX4:
                    //Swap pixels
                    for (int j = 0; j < buffer.Length; ++j) { buffer[j] = (byte)((buffer[j] << 4) | (buffer[j] >> 4)); }
                    break;
            }
        }
        private static int CLUTSwap(int i, bool linear)
        {
            if (!linear)
            {
                byte b = (byte)(i & 0x1F);
                if (b >= 8)
                {
                    if (b < 16) { i += 8; }
                    else if (b < 24) { i -= 8; }
                }
            }
            return i;
        }

        private Crazycatz00.Utility.BinaryStream file;
        private readonly ushort internalCount;
        private readonly byte alignment;

        public TIM2(Stream file)
        {
            if (!file.CanRead || !file.CanSeek) { throw new NotSupportedException("Cannot read or seek in stream."); }
            this.file = new Crazycatz00.Utility.BinaryStream(file);
            const uint TIM2Signature = 'T' | ('I' << 8) | ('M' << 16) | ('2' << 24);
            if (this.file.ReadUInt32() != TIM2Signature) { throw new InvalidDataException("Invalid signature."); }
            byte formatVer = this.file.ReadByte();
            if (formatVer != 3 && formatVer != 4 && formatVer != 0xEE) { throw new InvalidDataException("Invalid format version."); }
            switch (this.file.ReadByte())
            {
                case 0: alignment = 16; break;
                case 1: alignment = 128; break;
                default: throw new InvalidDataException("Invalid format alignment.");
            }
            internalCount = this.file.ReadUInt16();
        }
        protected override void Dispose(bool disposing)
        {
            this.file.Dispose();
            base.Dispose(disposing);
        }
        public override void parse()
        {
            if (this.bmps.Count != 0)
            {
                foreach (Bitmap bmp in this.bmps) { bmp.Dispose(); }
                this.bmps.Clear();
            }
            this.bmps.Capacity = this.internalCount;
            this.file.Seek(alignment, SeekOrigin.Begin);
            for (int i = 0; i < this.internalCount; ++i)
            {
                long lpos = this.file.Tell();
                uint len = parsePic(i, lpos);
                this.file.Seek(lpos + len, SeekOrigin.Begin);
            }
        }
        protected override void setBMPInternal(int index, ref Bitmap bmp)
        {
            if (!this.file.CanWrite) { throw new NotSupportedException("Stream is readonly."); }
            long offset = getPicOffset(index);
            this.file.Seek(offset + 4, SeekOrigin.Begin);
            uint clutSize = this.file.ReadUInt32(),
                imageSize = this.file.ReadUInt32();
            ushort headerSize = this.file.ReadUInt16();
            this.file.Seek(2 + 1 + 1, SeekOrigin.Current);
            CI_TYPE clutType = (CI_TYPE)this.file.ReadByte(),
                imageType = (CI_TYPE)this.file.ReadByte();
            ushort width = this.file.ReadUInt16(),
                height = this.file.ReadUInt16();
            if (bmp.Width != width || bmp.Height != height) { throw new NotSupportedException("New image has different dimensions."); }
            CI_TYPE clutTypeBase = BaseType(clutType);
            PixelFormat pf;
            switch (imageType)
            {
                case CI_TYPE.RGB16: pf = PixelFormat.Format16bppArgb1555; break;
                case CI_TYPE.RGB24: pf = PixelFormat.Format24bppRgb; break;
                case CI_TYPE.RGB32: pf = PixelFormat.Format32bppArgb; break;
                case CI_TYPE.INDEX4: pf = PixelFormat.Format4bppIndexed; break;
                case CI_TYPE.INDEX8: pf = PixelFormat.Format8bppIndexed; break;
                default: throw new NotSupportedException("Unsupported picture type.");
            }
            if (bmp.PixelFormat != pf)
            {
                switch (pf)
                {
                    case PixelFormat.Format32bppArgb:
                    case PixelFormat.Format24bppRgb:
                        // GDI+ doesn't support the alpha channel in 16bppArgb
                        var nbmp = new Bitmap(width, height, pf);
                        using (var gr = Graphics.FromImage(nbmp))
                        {
                            gr.DrawImage(bmp, new Rectangle(0, 0, width, height));
                        }
                        bmp = nbmp;
                        break;
                    default:
                        requestQuantize(ref bmp, pf);
                        break;
                }
            }
            {
                int buffSz = width * height;
                switch (imageType)
                {
                    case CI_TYPE.RGB16: buffSz *= 2; break;
                    case CI_TYPE.RGB24: buffSz *= 3; break;
                    case CI_TYPE.RGB32: buffSz *= 4; break;
                    case CI_TYPE.INDEX4: buffSz /= 2; break;
                }
                buffSz = (buffSz + 15) & ~15;
                if (buffSz > imageSize) { buffSz = (int)imageSize; }
                BitmapData pix = bmp.LockBits(System.Drawing.Rectangle.FromLTRB(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.ReadOnly, pf);
                byte[] buffer = new byte[buffSz];
                try { System.Runtime.InteropServices.Marshal.Copy(pix.Scan0, buffer, 0, buffSz); }
                finally { bmp.UnlockBits(pix); }
                InvertChannels(buffer, imageType, false);
                this.file.Seek(offset + headerSize, SeekOrigin.Begin);
                this.file.Write(buffer, 0, buffSz);
                if (buffSz != imageSize)
                {
                    this.file.Seek(offset + 4 + 4, SeekOrigin.Begin);
                    this.file.Write(buffSz);
                }
            }
            if (clutTypeBase != CI_TYPE.None)
            {
                ColorPalette palette = bmp.Palette;
                int mult = 0;
                switch (clutTypeBase)
                {
                    case CI_TYPE.RGB16: mult = 2; break;
                    case CI_TYPE.RGB24: mult = 3; break;
                    case CI_TYPE.RGB32: mult = 4; break;
                }
                int buffSz = palette.Entries.Length * mult;
                if (buffSz > clutSize) { buffSz = (int)clutSize; }
                int clutColors = buffSz / mult;
                bool isLinear = ((byte)clutType & 0x80) == 0x80 || (imageType == CI_TYPE.INDEX4 && ((byte)clutType & 0x40) == 0);
                byte[] buffer = new byte[buffSz];
                for (int i = 0; i < clutColors; ++i)
                {
                    int j = CLUTSwap(i, isLinear);
                    uint argb = (uint)palette.Entries[j].ToArgb();
                    switch (clutTypeBase)
                    {
                        case CI_TYPE.RGB16:
                            buffer[i * 2] = (byte)(((argb >> 6) & 0xE0u) | ((argb >> 19) & 0x1Fu));
                            buffer[i * 2 + 1] = (byte)(((argb >> 24) > 127 ? 0x80u : 0u) | ((argb >> 1) & 0x7Cu) | ((argb >> 14) & 0x03u));
                            palette.Entries[j] = Color.FromArgb((argb >> 24) > 127 ? 255 : 0, (int)(argb >> 16) & 0xF8, (int)(argb >> 8) & 0xF8, (int)argb & 0xF8);
                            break;
                        case CI_TYPE.RGB24:
                            buffer[i * 3] = (byte)(argb >> 16);
                            buffer[i * 3 + 1] = (byte)(argb >> 8);
                            buffer[i * 3 + 2] = (byte)argb;
                            palette.Entries[j] = Color.FromArgb(255, palette.Entries[j]);
                            break;
                        case CI_TYPE.RGB32:
                            buffer[i * 4 + 3] = (byte)Math.Ceiling((argb >> 24) / 2.0);
                            buffer[i * 4] = (byte)(argb >> 16);
                            buffer[i * 4 + 1] = (byte)(argb >> 8);
                            buffer[i * 4 + 2] = (byte)argb;
                            break;
                    }
                }
                this.file.Seek(offset + headerSize + imageSize, SeekOrigin.Begin);
                this.file.Write(buffer, 0, buffSz);
                if (buffSz != clutSize)
                {
                    this.file.Seek(offset + 4, SeekOrigin.Begin);
                    this.file.Write(buffSz);
                }
                bmp.Palette = palette;
            }
            this.file.Seek(offset + 4 + 4 + 4 + 2 + 2 + 1, SeekOrigin.Begin);
            this.file.Write((byte)1);
        }

        private long getPicOffset(int index)
        {
            long offset = alignment;
            for (int i = 0; i < index; ++i)
            {
                this.file.Seek(offset, SeekOrigin.Begin);
                offset += this.file.ReadUInt32();
            }
            return offset;
        }
        private uint parsePic(int index, long offset)
        {
#if DEBUG
            uint totalSize, clutSize, imageSize;
            ushort headerSize, clutColors, width, height;
            CI_TYPE clutType, imageType;
            Debug.WriteLine(string.Format("---TIM2---\r\n  TotalSize: {0}\r\n  CLUTSize: {1}\r\n  ImageSize: {2}\r\n  HeaderSize: {3}\r\n  CLUTColors: {4}\r\n  Format: {5}\r\n  MipMapCount: {6}\r\n  CLUTType: {7}\r\n  ImageType: {8}\r\n  Width: {9}\r\n  Height: {10}\r\n  Tex0: {11}\r\n  Tex1: {12}\r\n  TexaFbaPabe: {13}\r\n  TexCLUT: {14}",
                totalSize = this.file.ReadUInt32(),
                clutSize = this.file.ReadUInt32(),
                imageSize = this.file.ReadUInt32(),
                headerSize = this.file.ReadUInt16(),
                clutColors = this.file.ReadUInt16(),
                this.file.ReadByte(),
                this.file.ReadByte(),
                clutType = (CI_TYPE)this.file.ReadByte(),
                imageType = (CI_TYPE)this.file.ReadByte(),
                width = this.file.ReadUInt16(),
                height = this.file.ReadUInt16(),
                this.file.ReadUInt64(),
                this.file.ReadUInt64(),
                this.file.ReadUInt32(),
                this.file.ReadUInt32()
            ));
#else
            uint totalSize = this.file.ReadUInt32(),
                clutSize = this.file.ReadUInt32(),
                imageSize = this.file.ReadUInt32();
            ushort headerSize = this.file.ReadUInt16(),
                clutColors = this.file.ReadUInt16();
            this.file.Seek(2, SeekOrigin.Current);
            CI_TYPE clutType = (CI_TYPE)this.file.ReadByte(),
                imageType = (CI_TYPE)this.file.ReadByte();
            ushort width = this.file.ReadUInt16(),
                height = this.file.ReadUInt16();
#endif
            CI_TYPE clutTypeBase = BaseType(clutType);
            PixelFormat pf;
            int buffSz;
            switch (imageType)
            {
                case CI_TYPE.RGB16: pf = PixelFormat.Format16bppArgb1555; buffSz = 0; break;
                case CI_TYPE.RGB24: pf = PixelFormat.Format24bppRgb; buffSz = 0; break;
                case CI_TYPE.RGB32: pf = PixelFormat.Format32bppArgb; buffSz = 0; break;
                case CI_TYPE.INDEX4: pf = PixelFormat.Format4bppIndexed; buffSz = 16; break;
                case CI_TYPE.INDEX8: pf = PixelFormat.Format8bppIndexed; buffSz = 256; break;
                default: throw new NotSupportedException("Unsupported picture type.");
            }
            if (clutColors > buffSz) { clutColors = (ushort)buffSz; }
            Bitmap bmp = new Bitmap(width, height, pf);
            {
                this.file.Seek(offset + headerSize, SeekOrigin.Begin);
                buffSz = width * height;
                switch (imageType)
                {
                    case CI_TYPE.RGB16: buffSz *= 2; break;
                    case CI_TYPE.RGB24: buffSz *= 3; break;
                    case CI_TYPE.RGB32: buffSz *= 4; break;
                    case CI_TYPE.INDEX4: buffSz /= 2; break;
                }
                buffSz = (buffSz + 15) & ~15;
                if (buffSz > imageSize) { buffSz = (int)imageSize; }
                byte[] buffer = this.file.ReadBytes(buffSz);
                InvertChannels(buffer, imageType, true);
                BitmapData pix = bmp.LockBits(System.Drawing.Rectangle.FromLTRB(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.WriteOnly, pf);
                try { System.Runtime.InteropServices.Marshal.Copy(buffer, 0, pix.Scan0, buffSz); }
                finally { bmp.UnlockBits(pix); }
            }
            //CLUT base type
            if (clutTypeBase != CI_TYPE.None && clutColors != 0)
            {
                this.file.Seek(offset + headerSize + imageSize, SeekOrigin.Begin);
                buffSz = clutColors;
                switch (clutTypeBase)
                {
                    case CI_TYPE.RGB16: buffSz *= 2; break;
                    case CI_TYPE.RGB24: buffSz *= 3; break;
                    case CI_TYPE.RGB32: buffSz *= 4; break;
                }
                if (buffSz > clutSize) { buffSz = (int)clutSize; }
                bool isLinear = ((byte)clutType & 0x80) == 0x80 || (imageType == CI_TYPE.INDEX4 && ((byte)clutType & 0x40) == 0);
                byte[] buffer = this.file.ReadBytes(buffSz);
                ColorPalette palette = bmp.Palette;
                switch (clutTypeBase)
                {
                    case CI_TYPE.RGB16:
                        for (int i = 0; i < clutColors; ++i)
                        {
                            buffSz = (buffer[i * 2 + 1] << 8) | buffer[i * 2];
                            palette.Entries[CLUTSwap(i, isLinear)] = Color.FromArgb((buffSz & 0x8000) != 0 ? 255 : 0, ((buffSz << 3) & 0xF8), ((buffSz >> 2) & 0xF8), ((buffSz >> 7) & 0xF8));
                        }
                        break;
                    case CI_TYPE.RGB24:
                        for (int i = 0; i < clutColors; ++i)
                        {
                            palette.Entries[CLUTSwap(i, isLinear)] = Color.FromArgb(255, buffer[i * 3], buffer[i * 3 + 1], buffer[i * 3 + 2]);
                        }
                        break;
                    case CI_TYPE.RGB32:
                        for (int i = 0; i < clutColors; ++i)
                        {
                            palette.Entries[CLUTSwap(i, isLinear)] = Color.FromArgb((int)Math.Min(buffer[i * 4 + 3] * 2, 255), buffer[i * 4], buffer[i * 4 + 1], buffer[i * 4 + 2]);
                            Debug.WriteLineIf(buffer[(i * 4) + 3] > 128, "Transparency before transform is over 128: " + buffer[(i * 4) + 3]);
                        }
                        break;
                }
                bmp.Palette = palette;
            }
            else if (imageType == CI_TYPE.INDEX8 || imageType == CI_TYPE.INDEX4) { throw new InvalidDataException("The image is indexed but there is no palette."); }
            this.bmps.Add(bmp);
            return totalSize;
        }

    }
}
