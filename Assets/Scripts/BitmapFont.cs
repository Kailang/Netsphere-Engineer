using System.Collections.Generic;
using System.IO;
using System;
using NUnit.Framework;

namespace ImGui {
	public class BitmapFont : Streamable {
		// fontSize		2	int		0
		public readonly short fontSize;
		// bitField		1	bits	2	bit 0: smooth, bit 1: unicode, bit 2: italic, bit 3: bold, bit 4: fixedHeigth, bits 5-7: reserved
		// charSet		1	uint	3
		// stretchH		2	uint	4
		// aa			1	uint	6
		// paddingUp	1	uint	7
		// paddingRight	1	uint	8
		// paddingDown	1	uint	9
		// paddingLeft	1	uint	10
		// spacingHoriz	1	uint	11
		public readonly byte spacingHoriz;
		// spacingVert	1	uint	12
		public readonly byte spacingVert;
		// outline		1	uint	13	added with version 2
		public readonly byte outline;
		// fontName		n+1	string	14	null terminated string with length n
		public readonly string fontName;

		// lineHeight	2	uint	0
		// base			2	uint	2
		// scaleW		2	uint	4
		// scaleH		2	uint	6
		public readonly ushort scaleW, scaleH;
		// pages		2	uint	8
		// bitField		1	bits	10	bits 0-6: reserved, bit 7: packed
		// alphaChnl	1	uint	11
		// redChnl		1	uint	12
		// greenChnl	1	uint	13
		// blueChnl		1	uint	14

		// pageNames	p*(n+1)	strings	0	p null terminated strings, each with length n
		public readonly string[] pageNames;

		public class Glyph : Streamable {
			// id			4	uint	0+c*20	These fields are repeated until all characters have been described
			public readonly uint id;
			// x			2	uint	4+c*20
			// y			2	uint	6+c*20
			public readonly uint x, y;
			// width		2	uint	8+c*20
			// height		2	uint	10+c*20
			public readonly uint width, height;
			// xoffset		2	int		12+c*20
			// yoffset		2	int		14+c*20
			public readonly int xOffset, yOffset;
			// xadvance		2	int		16+c*20
			public readonly int xAdvance;
			// page			1	uint	18+c*20
			// chnl			1	uint	19+c*20

			internal Glyph(Stream stream) : base(stream) {
				// id			4	uint	0+c*20	These fields are repeated until all characters have been described
				id = ReadUInt32();
				// x			2	uint	4+c*20
				// y			2	uint	6+c*20
				x = ReadUInt16();
				y = ReadUInt16();
				// width		2	uint	8+c*20
				// height		2	uint	10+c*20
				width = ReadUInt16();
				height = ReadUInt16();
				// xoffset		2	int		12+c*20
				// yoffset		2	int		14+c*20
				xOffset = ReadInt16();
				yOffset = ReadInt16();
				// xadvance		2	int		16+c*20
				xAdvance = ReadInt16();
				// page			1	uint	18+c*20
				ReadByte();
				// chnl			1	uint	19+c*20
				ReadByte();
			}
		}

		Dictionary<uint, Glyph> dict_ = new Dictionary<uint, Glyph>();

		public BitmapFont(Stream stream) : base(stream) {
			Assert.AreEqual('B', ReadChar());
			Assert.AreEqual('M', ReadChar());
			Assert.AreEqual('F', ReadChar());
			Assert.AreEqual(3, ReadByte());

			Assert.AreEqual(1, ReadByte());
			var length = ReadInt32();

			// fontSize		2	int		0
			fontSize = ReadInt16();
			// bitField		1	bits	2	bit 0: smooth, bit 1: unicode, bit 2: italic, bit 3: bold, bit 4: fixedHeigth, bits 5-7: reserved
			ReadByte();
			// charSet		1	uint	3
			ReadByte();
			// stretchH		2	uint	4
			ReadUInt16();
			// aa			1	uint	6
			ReadByte();
			// paddingUp	1	uint	7
			ReadByte();
			// paddingRight	1	uint	8
			ReadByte();
			// paddingDown	1	uint	9
			ReadByte();
			// paddingLeft	1	uint	10
			ReadByte();
			// spacingHoriz	1	uint	11
			spacingHoriz = ReadByte();
			// spacingVert	1	uint	12
			spacingVert = ReadByte();
			// outline		1	uint	13	added with version 2
			outline = ReadByte();
			// fontName		n+1	string	14	null terminated string with length n
			fontName = ReadString(length - 14);

//		Debug.Log(fontName);

			Assert.AreEqual(2, ReadByte());
			length = ReadInt32();

			// lineHeight	2	uint	0
			ReadUInt16();
			// base			2	uint	2
			ReadUInt16();
			// scaleW		2	uint	4
			// scaleH		2	uint	6
			scaleW = ReadUInt16();
			scaleH = ReadUInt16();
			// pages		2	uint	8
			ReadUInt16();
			// bitField		1	bits	10	bits 0-6: reserved, bit 7: packed
			ReadByte();
			// alphaChnl	1	uint	11
			ReadByte();
			// redChnl		1	uint	12
			ReadByte();
			// greenChnl	1	uint	13
			ReadByte();
			// blueChnl		1	uint	14
			ReadByte();

			Assert.AreEqual(3, ReadByte());
			length = ReadInt32();
			pageNames = ReadString(length).Split(new [] { '\0' }, StringSplitOptions.RemoveEmptyEntries);
//		foreach (var n in pageNames) {
//			Debug.Log(n);
//		}

			Assert.AreEqual(4, ReadByte());
			length = ReadInt32();
			for (int i = 0; i < length; i += 20) {
				var glyph = new Glyph(stream);
				dict_.Add(glyph.id, glyph);
			}
//		Debug.Log(dict_.Count);
		}

		public bool HasGlyph(uint id) {
			return dict_.ContainsKey(id);
		}

		public Glyph GetGlyph(uint id) {
			return dict_[id];
		}
	}

	public class Streamable {
		protected readonly Stream stream_;

		protected Streamable(Stream stream) {
			stream_ = stream;
		}

		protected void Skip(int val) {
			stream_.Position += val;
		}

		protected Char ReadChar() {
			return (char)stream_.ReadByte();
		}

		protected Byte ReadByte() {
			return (byte)stream_.ReadByte();
		}

		protected string ReadString(int count) {
			var buffer = new byte[count];
			stream_.Read(buffer, 0, count);
			return System.Text.Encoding.UTF8.GetString(buffer).Trim();
		}

		protected Int16 ReadInt16() {
			return (short)(stream_.ReadByte() | (sbyte)stream_.ReadByte() << 8);
		}

		protected Int32 ReadInt32() {
			return stream_.ReadByte() | stream_.ReadByte() << 8 | stream_.ReadByte() << 16 | (sbyte)stream_.ReadByte() << 24;
		}

		protected UInt16 ReadUInt16() {
			return (ushort)(stream_.ReadByte() | stream_.ReadByte() << 8);
		}

		protected UInt32 ReadUInt32() {
			return (uint)(stream_.ReadByte() | stream_.ReadByte() << 8 | stream_.ReadByte() << 16 | stream_.ReadByte() << 24);
		}
	}
}