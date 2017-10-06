using System.Collections.Generic;
using UnityEngine;

namespace ImGui {
	public class DrawList {
		static readonly Vector2 kWhiteUv = Vector2.up;

		readonly List<Vector3> vertexList = new List<Vector3>();
		readonly List<Vector2> uvList = new List<Vector2>();
		readonly List<Color> colorList = new List<Color>();
		readonly List<int> triangleList = new List<int>();

		readonly Mesh mesh;

		DrawContext context;

		public DrawList(Mesh mesh) {
			this.mesh = mesh;
		}

		public void ClearMesh() {
			mesh.Clear();

			vertexList.Clear();
			uvList.Clear();
			colorList.Clear();
			triangleList.Clear();
		}

		public Mesh ApplyMesh() {
			mesh.vertices = vertexList.ToArray();
			mesh.colors = colorList.ToArray();
			mesh.uv = uvList.ToArray();
			mesh.triangles = triangleList.ToArray();

			return mesh;
		}

		public void SetDrawingContext(DrawContext context) {
			this.context = context;
		}
			
		public Vector2 CalcTextSize(BitmapFont font, string text, float size) {
			float scale = size / font.fontSize;

			text = text.Replace("\t", "    ");

			float x = 0, y = 0, xMax = x;
			foreach (var c in text) {
				uint uc = (uint)c;

				if (c == '\n') {
					x = 0;
					y -= size;
				} else if (font.HasGlyph(uc)) {
					x += (font.GetGlyph(uc).xAdvance + font.spacingHoriz) * scale;
					if (x > xMax) xMax = x;
				}
			}

			return new Vector2(xMax - font.spacingHoriz * scale, -(y - size));
		}

		public Vector2 AddText(BitmapFont font, Color color, Vector2 pos, string text, float size, bool wrap = false) {
			float scale = size / font.fontSize;
			Vector2 textureScale = new Vector2(1f / font.scaleW, 1f / font.scaleH);

			text = text.Replace("\t", "    ");

			float x = pos.x, y = pos.y, xMax = x;
			foreach (var c in text) {
				uint uc = (uint)c;

				if (c == '\n') {
					x = pos.x;
					y -= size;
				} else if (font.HasGlyph(uc)) {
					BitmapFont.Glyph g = font.GetGlyph(uc);

					if (wrap && x + (g.xOffset + g.width) * scale > context.right) {  // Wrap text
						x = pos.x;
						y -= size;
					}
					
					if (c != ' ') {
						float x1 = x + g.xOffset * scale, x2 = x1 + g.width * scale;
						float y1 = y + g.yOffset * scale, y2 = y1 - g.height * scale;

						float s1 = g.x * textureScale.x, s2 = (g.x + g.width) * textureScale.x;
						float t1 = g.y * textureScale.y, t2 = (g.y + g.height) * textureScale.y;

						AddQuad(color,
							new Vector2(x1, y1), new Vector2(x2, y1), 
							new Vector2(x2, y2), new Vector2(x1, y2), 

							new Vector2(s1, 1f - t1), new Vector2(s2, 1f - t1), 
							new Vector2(s2, 1f - t2), new Vector2(s1, 1f - t2));
					}

					x += (g.xAdvance + font.spacingHoriz) * scale;
					if (x > xMax) xMax = x;
				}
			}

//		AddRectFill(ColorPreset.Debug, pos, new Vector2(xMax - font.spacingHoriz * scale, y - size));
			return new Vector2(xMax - font.spacingHoriz * scale, y - size);
		}

		public void AddRoundedRectFill(Color color, Vector2 a, Vector2 b, float radius, RectCorner corners = RectCorner.All, int segments = 3) {
			bool roundTopLeft = ((corners & RectCorner.TopLeft) != 0);
			bool roundTopRight = ((corners & RectCorner.TopRight) != 0);
			bool roundBottomLeft = ((corners & RectCorner.BottomLeft) != 0);
			bool roundBottomRight = ((corners & RectCorner.BottomRight) != 0);

			float xMin0 = Mathf.Min(a.x, b.x), xMax0 = Mathf.Max(a.x, b.x);
			float yMin0 = Mathf.Min(a.y, b.y), yMax0 = Mathf.Max(a.y, b.y);

			Vector2 v00 = new Vector2(xMin0, yMin0), v01 = new Vector2(xMin0, yMax0);
			Vector2 v02 = new Vector2(xMax0, yMax0), v03 = new Vector2(xMax0, yMin0);

			float xMin1 = xMin0 + radius, xMax1 = xMax0 - radius;
			float yMin1 = yMin0 + radius, yMax1 = yMax0 - radius;

			Vector2 v10 = new Vector2(xMin1, yMin1), v11 = new Vector2(xMin1, yMax1);
			Vector2 v12 = new Vector2(xMax1, yMax1), v13 = new Vector2(xMax1, yMin1);

			/**
			 * v01 -------- v02
			 * |  v11 - v12 |
			 * |  |     | 1 | 0
			 * |  v10 - v13 |
			 * v00 -------- v03
			 */

			AddQuad(color, 
				new Vector2(xMin1, yMin0),
				new Vector2(xMin1, yMax0),
				new Vector2(xMax1, yMax0),
				new Vector2(xMax1, yMin0));

			AddQuad(color, 
				roundBottomLeft ? new Vector2(xMin0, yMin1) : v00,
				roundTopLeft ? new Vector2(xMin0, yMax1) : v01,
				roundTopLeft ? v11 : new Vector2(xMin1, yMax0),
				roundBottomLeft ? v10 : new Vector2(xMin1, yMin0));

			AddQuad(color, 
				roundBottomRight ? v13 : new Vector2(xMax1, yMin0),
				roundTopRight ? v12 : new Vector2(xMax1, yMax0),
				roundTopRight ? new Vector2(xMax0, yMax1) : v02,
				roundBottomRight ? new Vector2(xMax0, yMin1) : v03);

			if (roundBottomLeft) AddFanFill(color, v10, radius, 6, 6 + segments + 1, segments * 4);
			if (roundTopLeft) AddFanFill(color, v11, radius, 3, 3 + segments + 1, segments * 4);
			if (roundTopRight) AddFanFill(color, v12, radius, 0, 0 + segments + 1, segments * 4);
			if (roundBottomRight) AddFanFill(color, v13, radius, 9, 9 + segments + 1, segments * 4);
		}

		public void AddFanFill(Color color, Vector2 center, float radius, int begin = 0, int end = 12, int segments = 12) {
			int size = end - begin;
			var vs = new Vector3[size + 1];
			for (int i = begin, j = 0; i < end; i++, j++) {
				float a = (float)i / segments * 2 * Mathf.PI;
				vs[j].x = Mathf.Cos(a);
				vs[j].y = Mathf.Sin(a);
				vs[j] = (Vector3)center + (vs[j] * radius);
			}
			vs[size] = center;

			int vi = AddVertices(color, vs);
			for (int i = 0; i < size - 1; i++) {
				AddTriangle(
					vi + size,
					vi + i + 1,
					vi + i);
			}
		}

		public void AddCircleFill(Color color, Vector2 center, float radius, int segments = 12) {
			AddFanFill(color, center, radius, 0, segments, segments);
		}

		public Vector2 AddRectFill(Color color, Vector2 a, Vector2 b) {
			float xMin = Mathf.Min(a.x, b.x), xMax = Mathf.Max(a.x, b.x);
			float yMin = Mathf.Min(a.y, b.y), yMax = Mathf.Max(a.y, b.y);

			AddQuad(color, 
				new Vector2(xMin, yMin),
				new Vector2(xMin, yMax),
				new Vector2(xMax, yMax),
				new Vector2(xMax, yMin));

			return new Vector2(xMax, yMin);
		}

		public void AddLine(Color color, Vector2 a, Vector2 b, float thickness = 0.1f) {
			Vector2 n = (b - a).normalized * thickness;
			Vector2 hn = new Vector2(n.y, -n.x) * 0.5f;

			AddQuad(color, a - hn, b - hn, b + hn, a + hn);
		}

		/**
		 * v1 - v2
		 * |  / |
		 * v0 - v3
		 */
		void AddQuad(Color color, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3) {
			if (ShouldClip(v0, v1, v2, v3)) return;

			int begin = AddVertices(color, v0, v1, v2, v3);
			AddTriangle(
				begin,
				begin + 1,
				begin + 2);

			AddTriangle(
				begin + 2,
				begin + 3,
				begin);
		}

		/**
		 * v1 - v2
		 * |  / |
		 * v0 - v3
		 */
		void AddQuad(Color color, Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3, Vector2 uv0, Vector2 uv1, Vector2 uv2, Vector2 uv3) {
			if (ShouldClip(v0, v1, v2, v3)) return;

			int begin = AddVertices(color, new [] { v0, v1, v2, v3 }, new [] { uv0, uv1, uv2, uv3 });
			AddTriangle(
				begin,
				begin + 1,
				begin + 2);

			AddTriangle(
				begin + 2,
				begin + 3,
				begin);
		}

		bool ShouldClip(params Vector2[] vs) {
			if (context == null) return true;

			foreach (var v in vs) {
				if (!context.Contains(v)) return true;
			}

			return false;
		}

		int AddVertices(Color color, params Vector3[] vs) {
			int begin = vertexList.Count;

			for (int i = 0; i < vs.Length; i++) {
				vertexList.Add(vs[i]);
				uvList.Add(kWhiteUv);
				colorList.Add(color);
			}

			return begin;
		}

		int AddVertices(Color color, Vector3[] vs, Vector2[] uvs) {
			int begin = vertexList.Count;

			for (int i = 0; i < vs.Length; i++) {
				vertexList.Add(vs[i]);
				uvList.Add(uvs[i]);
				colorList.Add(color);
			}

			return begin;
		}

		/**
		 * v1
		 * |  \
		 * v0 - v2
		 */
		void AddTriangle(int v0, int v1, int v2) {
			triangleList.Add(v0);
			triangleList.Add(v1);
			triangleList.Add(v2);
		}

		public void DrawGizmos() {
			for (int i = 0; i < vertexList.Count; i++) {
				Gizmos.color = colorList[i];
				Gizmos.DrawSphere(vertexList[i], 0.05f);
			}

			Gizmos.color = Color.green;
			for (int i = 0; i < triangleList.Count; i += 3) {
				DrawArrow(vertexList[triangleList[i]], vertexList[triangleList[i + 1]]);
				DrawArrow(vertexList[triangleList[i + 1]], vertexList[triangleList[i + 2]]);
				DrawArrow(vertexList[triangleList[i + 2]], vertexList[triangleList[i]]);
			}
		}

		static void DrawArrow(Vector3 begin, Vector3 end, float arrowHeadLength = 0.1f, float arrowHeadAngle = 30.0f) {
			var direction = end - begin;
			arrowHeadLength *= Mathf.Pow(direction.magnitude, 0.5f);

			Gizmos.color = Color.green;
			Gizmos.DrawRay(begin, direction);

			Vector3 right = arrowHeadLength * (Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1));
			Vector3 left = arrowHeadLength * (Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1));

			begin += direction * 0.5f;

			Gizmos.color = Color.cyan;
			Gizmos.DrawRay(begin, right);
			Gizmos.DrawRay(begin, left);
			Gizmos.DrawLine(begin + right, begin + left);
		}

		public void PrimitiveTest(BitmapFont font) {
			context = new DrawContext(new Vector2(-1000, 1000), new Vector2(1000, -1000));

			AddLine(ColorPreset.WindowBg, Vector2.zero, Vector2.right);
			AddLine(ColorPreset.Border, Vector2.zero, Vector2.one);
			AddRectFill(ColorPreset.WindowBg, Vector2.zero, Vector2.one);
			AddFanFill(ColorPreset.Button, Vector2.one / 2 + Vector2.right / 2, 0.5f, 0, 4);
			AddFanFill(ColorPreset.Button, Vector2.one / 2 + Vector2.right / 2, 0.5f, 4, 8);
			AddFanFill(ColorPreset.Button, Vector2.one / 2 + Vector2.right / 2, 0.5f, 8, 12);

			AddRectFill(ColorPreset.WindowBg, new Vector2(-40, 0), new Vector2(-40, 0) + Style.WindowMinSize);
			AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(-40, 40), new Vector2(-40, 40) + Style.WindowMinSize, Style.WindowRounding, RectCorner.All);
			
			AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(0, 0), new Vector2(0, 0) + Style.WindowMinSize, Style.WindowRounding, RectCorner.BottomLeft);
			AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(40, 0), new Vector2(40, 0) + Style.WindowMinSize, Style.WindowRounding, RectCorner.TopLeft);
			AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(0, 40), new Vector2(0, 40) + Style.WindowMinSize, Style.WindowRounding, RectCorner.TopRight);
			AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(40, 40), new Vector2(40, 40) + Style.WindowMinSize, Style.WindowRounding, RectCorner.BottomRight);

			AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(80, 0), new Vector2(80, 0) + Style.WindowMinSize, Style.WindowRounding, RectCorner.Bottom);
			AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(80, 40), new Vector2(80, 40) + Style.WindowMinSize, Style.WindowRounding, RectCorner.Top);
			AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(120, 0), new Vector2(120, 0) + Style.WindowMinSize, Style.WindowRounding, RectCorner.Right);
			AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(120, 40), new Vector2(120, 40) + Style.WindowMinSize, Style.WindowRounding, RectCorner.Left);

			AddFanFill(ColorPreset.Button, Vector2.left, 20f, 0, 19, 36);
			AddText(font, ColorPreset.Text, Vector2.zero, "Hellow, world!\nFrom Dear ImGUI\nfor (int i = 0; i < count; i++) {\n\tconsole.log();\n}", 13);
		}
	}
}

