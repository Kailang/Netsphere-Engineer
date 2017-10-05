using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Test : MonoBehaviour {
	static readonly Vector2 kWhiteUv = Vector2.up;
	const float kPixelsPerUnit = 10;

	const int kNullId = -1;
	public Vector2 pointerPosition;
	public bool isPointerPressed, isPointerHeld, isPointerRepeating;
	public float pointerPressedTime, time;

	static class ColorPreset {
		public static Color Text = new Color(0.90f, 0.90f, 0.90f, 1.00f);
		public static Color TextDisabled = new Color(0.60f, 0.60f, 0.60f, 1.00f);
		public static Color WindowBg = new Color(0.00f, 0.00f, 0.00f, 0.70f);
		public static Color ChildWindowBg = new Color(0.00f, 0.00f, 0.00f, 0.00f);
		public static Color PopupBg = new Color(0.05f, 0.05f, 0.10f, 0.90f);
		public static Color Border = new Color(0.70f, 0.70f, 0.70f, 0.40f);
		public static Color BorderShadow = new Color(0.00f, 0.00f, 0.00f, 0.00f);
		public static Color FrameBg = new Color(0.80f, 0.80f, 0.80f, 0.30f);
		// Background of checkbox, radio button, plot, slider, text input
		public static Color FrameBgHovered = new Color(0.90f, 0.80f, 0.80f, 0.40f);
		public static Color FrameBgActive = new Color(0.90f, 0.65f, 0.65f, 0.45f);
		public static Color TitleBg = new Color(0.27f, 0.27f, 0.54f, 0.83f);
		public static Color TitleBgCollapsed = new Color(0.40f, 0.40f, 0.80f, 0.20f);
		public static Color TitleBgActive = new Color(0.32f, 0.32f, 0.63f, 0.87f);
		public static Color MenuBarBg = new Color(0.40f, 0.40f, 0.55f, 0.80f);
		public static Color ScrollbarBg = new Color(0.20f, 0.25f, 0.30f, 0.60f);
		public static Color ScrollbarGrab = new Color(0.40f, 0.40f, 0.80f, 0.30f);
		public static Color ScrollbarGrabHovered = new Color(0.40f, 0.40f, 0.80f, 0.40f);
		public static Color ScrollbarGrabActive = new Color(0.80f, 0.50f, 0.50f, 0.40f);
		public static Color ComboBg = new Color(0.20f, 0.20f, 0.20f, 0.99f);
		public static Color CheckMark = new Color(0.90f, 0.90f, 0.90f, 0.50f);
		public static Color SliderGrab = new Color(1.00f, 1.00f, 1.00f, 0.30f);
		public static Color SliderGrabActive = new Color(0.80f, 0.50f, 0.50f, 1.00f);
		public static Color Button = new Color(0.67f, 0.40f, 0.40f, 0.60f);
		public static Color ButtonHovered = new Color(0.67f, 0.40f, 0.40f, 1.00f);
		public static Color ButtonActive = new Color(0.80f, 0.50f, 0.50f, 1.00f);
		public static Color Header = new Color(0.40f, 0.40f, 0.90f, 0.45f);
		public static Color HeaderHovered = new Color(0.45f, 0.45f, 0.90f, 0.80f);
		public static Color HeaderActive = new Color(0.53f, 0.53f, 0.87f, 0.80f);
		public static Color Separator = new Color(0.50f, 0.50f, 0.50f, 1.00f);
		public static Color SeparatorHovered = new Color(0.60f, 0.60f, 0.70f, 1.00f);
		public static Color SeparatorActive = new Color(0.70f, 0.70f, 0.90f, 1.00f);
		public static Color ResizeGrip = new Color(1.00f, 1.00f, 1.00f, 0.30f);
		public static Color ResizeGripHovered = new Color(1.00f, 1.00f, 1.00f, 0.60f);
		public static Color ResizeGripActive = new Color(1.00f, 1.00f, 1.00f, 0.90f);
		public static Color CloseButton = new Color(0.50f, 0.50f, 0.90f, 0.50f);
		public static Color CloseButtonHovered = new Color(0.70f, 0.70f, 0.90f, 0.60f);
		public static Color CloseButtonActive = new Color(0.70f, 0.70f, 0.70f, 1.00f);
		public static Color PlotLines = new Color(1.00f, 1.00f, 1.00f, 1.00f);
		public static Color PlotLinesHovered = new Color(0.90f, 0.70f, 0.00f, 1.00f);
		public static Color PlotHistogram = new Color(0.90f, 0.70f, 0.00f, 1.00f);
		public static Color PlotHistogramHovered = new Color(1.00f, 0.60f, 0.00f, 1.00f);
		public static Color TextSelectedBg = new Color(0.00f, 0.00f, 1.00f, 0.35f);
		public static Color ModalWindowDarkening = new Color(0.20f, 0.20f, 0.20f, 0.35f);

		public static Color Debug = new Color(1, 0, 1, 0.1f);
	}

	static class Style {
		public static Vector2 WindowPadding = new Vector2(8, 8);
		// Padding within a window
		public static Vector2 WindowMinSize = new Vector2(64, 64);
		// Minimum window size
		public static float WindowRounding = 9.0f;
		// Radius of window corners rounding. Set to 0.0f to have rectangular windows
		public static Vector2 WindowTitleAlign = new Vector2(0.0f, 0.5f);
		// Alignment for title bar text
		public static float ChildWindowRounding = 0.0f;
		// Radius of child window corners rounding. Set to 0.0f to have rectangular child windows
		public static Vector2 FramePadding = new Vector2(4, 3);
		// Padding within a framed rectangle (used by most widgets)
		public static float FrameRounding = 0.0f;
		// Radius of frame corners rounding. Set to 0.0f to have rectangular frames (used by most widgets).
		public static Vector2 ItemSpacing = new Vector2(8, 4);
		// Horizontal and vertical spacing between widgets/lines
		public static Vector2 ItemInnerSpacing = new Vector2(4, 4);
		// Horizontal and vertical spacing between within elements of a composed widget (e.g. a slider and its label)
		public static float TouchExtraPadding = 0.1f;
		// Expand reactive bounding box for touch-based system where touch position is not accurate enough. Unfortunately we don't sort widgets so priority on overlap will always be given to the first widget. So don't grow this too much!
		public static float IndentSpacing = 21.0f;
		// Horizontal spacing when e.g. entering a tree node. Generally == (FontSize + FramePadding.x*2).
		public static float ColumnsMinSpacing = 6.0f;
		// Minimum horizontal spacing between two columns
		public static float ScrollbarSize = 16.0f;
		// Width of the vertical scrollbar, Height of the horizontal scrollbar
		public static float ScrollbarRounding = 9.0f;
		// Radius of grab corners rounding for scrollbar
		public static float GrabMinSize = 10.0f;
		// Minimum width/height of a grab box for slider/scrollbar
		public static float GrabRounding = 0.0f;
		// Radius of grabs corners rounding. Set to 0.0f to have rectangular slider grabs.
		public static Vector2 ButtonTextAlign = new Vector2(0.5f, 0.5f);
		// Alignment of button text when button is larger than text.
		public static Vector2 DisplayWindowPadding = new Vector2(22, 22);
		// Window positions are clamped to be visible within the display area by at least this amount. Only covers regular windows.
		public static Vector2 DisplaySafeAreaPadding = new Vector2(4, 4);
		// If you cannot see the edge of your screen (e.g. on a TV) increase the safe area padding. Covers popups/tooltips as well regular windows.
		public static bool AntiAliasedLines = true;
		// Enable anti-aliasing on lines/borders. Disable if you are really short on CPU/GPU.
		public static bool AntiAliasedShapes = true;
		// Enable anti-aliasing on filled shapes (rounded rectangles, circles, etc.)
		public static float CurveTessellationTol = 1.25f;
		// Tessellation tolerance. Decrease for highly tessellated curves (higher quality, more polygons), increase to reduce quality.

		public static float FontSize = 13;
		public static float TitleHeight { get { return FontSize + FramePadding.y * 2; } }
		public static float KeyRepeatDelay = 0.250f;
		public static float KeyRepeatRate = 0.050f;
	}

	readonly List<Vector3> vertexList = new List<Vector3>();
	readonly List<Vector2> uvList = new List<Vector2>();
	readonly List<Color> colorList = new List<Color>();
	readonly List<int> triangleList = new List<int>();

	public void ClearMesh() {
		mesh.Clear();

		vertexList.Clear();
		uvList.Clear();
		colorList.Clear();
		triangleList.Clear();
	}

	/**
	* (0, 0) -> (w, 0)
	*  |         |
	*  v         v
	* (0, h) -> (w, h)
	* 
	* root -> max
	*  |       |
	*  v       v
	* min --> span
	*/
	class DrawingContext {
		public readonly Vector2 root;
		public readonly float width, height;

		public float left { get { return root.x; } }
		public float right { get { return root.x + width; } }
		public float top { get { return root.y; } }
		public float bottom { get { return root.y + height; } }

		public Vector2 size { get { return new Vector2(width, height); } }
		public Vector2 span { get { return new Vector2(root.x + width, root.y + height); } }

		public Vector2 min { get { return new Vector2(root.x, root.y + height); } }
		public Vector2 max { get { return new Vector2(root.x + width, root.y); } }

		// TODO: Finish this...
		Vector2 contentSize_, cursorPrev_;
		float lineHeightPrev_;

		public Vector2 cursor { get; private set; }

		public float padLeft { get; private set; }
		public float padTop { get; private set; }
		public float padRight { get; private set; }
		public float padBottom { get; private set; }

		public DrawingContext(Vector2 root, Vector2 extent) {
			this.root = root;
			cursor = root;

			width = Mathf.Abs(extent.x);
			height = -Mathf.Abs(extent.y);
		}

		public Vector2 FromTop(float height) {
			return new Vector2(right, top - height);
		}

		public Vector2 FromBottom(float height) {
			return new Vector2(right, bottom + height);
		}

		public Vector2 FromLeft(float width) {
			return new Vector2(left + width, bottom);
		}

		public Vector2 FromRight(float width) {
			return new Vector2(right - width, bottom);
		}

		public Vector2 FromRoot(Vector2 extent) {
			return new Vector2(root.x + Mathf.Abs(extent.x), root.y - Mathf.Abs(extent.y));
		}

		public Vector2 FromCursor(Vector2 extent) {
			return new Vector2(cursor.x + Mathf.Abs(extent.x), cursor.y - Mathf.Abs(extent.y));
		}

		/**
		 * root
		 * I
		 * I  cursor ---
		 * I  |        |
		 *    -------- pos
		 * <ItemSpacing.y>
		 * <--
		 * 
		 * root
		 * I
		 * I  cursor ---
		 * I  |        |
		 * I  -------- pos
		 * I
		 * <ItemSpacing.y>
		 * <--
		 */
		public void NextItem(Vector2 pos) {
			lineHeightPrev_ = Mathf.Min(pos.y, lineHeightPrev_);
			cursorPrev_ = new Vector2(pos.x, cursor.y);
			cursor = new Vector2(root.x, lineHeightPrev_ - Style.ItemSpacing.y);
		}

		/**
		 * root
		 *    ---------cursorPre_        <--
		 *    |        | <ItemSpacing.x> |
		 *    ----------                 
		 * <ItemSpacing.y>
		 * cursor
		 */
		public void BackCursor() {
			cursor = new Vector2(cursorPrev_.x + Style.ItemSpacing.x, cursorPrev_.y);
		}

		public bool Contains(Vector2 point) {
			const float e = 0.01f;

			return left - e < point.x && point.x < right + e && bottom - e < point.y && point.y < top + e;
		}

		public DrawingContext Pad(float x, float y) {
			return Pad(x, y, x, y);
		}

		public DrawingContext Pad(float left, float top, float right, float bottom) {
			padLeft = left;
			padTop = top;
			padRight = right;
			padBottom = bottom;

			var r = new Vector2(root.x + left, root.y - top);
			var s = new Vector2(width - left - right, height + top + bottom);

			if (s.x <= 0 || s.y >= 0) return new DrawingContext(r, Vector2.zero);
			return new DrawingContext(r, s);
		}
	}

	readonly Dictionary<string, Vector2> sizeDict = new Dictionary<string, Vector2>();
	readonly Stack<DrawingContext> drawingContextStack = new Stack<DrawingContext>();

	Mesh mesh;

	UiFont font;

	Vector2 lastCursorPosition, cursorPosition;

	public void OnValidate() {
		font = null;
		using (var stream = new System.IO.MemoryStream(Resources.Load<TextAsset>("ProggyClean").bytes)) {
			font = new UiFont(stream);
		}

		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "UiMesh";

//		AddLine(ColorPreset.WindowBg, Vector2.zero, Vector2.up);
//		AddLine(ColorPreset.WindowBg, Vector2.zero, Vector2.right);
//		AddLine(ColorPreset.Border, Vector2.zero, Vector2.one);
//		AddRectFill(ColorPreset.WindowBg, Vector2.zero, Vector2.one);
//		AddCircleFill(ColorPreset.Button, Vector2.one / 2 + Vector2.right / 2, 0.5f, 0, 4);
//		AddCircleFill(ColorPreset.Button, Vector2.one / 2 + Vector2.right / 2, 0.5f, 4, 8);
//		AddCircleFill(ColorPreset.Button, Vector2.one / 2 + Vector2.right / 2, 0.5f, 8, 12);

//		AddRectFill(ColorPreset.WindowBg, new Vector2(-40, 0), new Vector2(-40, 0) + Style.WindowMinSize);
//		AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(-40, 40), new Vector2(-40, 40) + Style.WindowMinSize, Style.WindowRounding, RoundedCorner.All);
//
//		AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(0, 0), new Vector2(0, 0) + Style.WindowMinSize, Style.WindowRounding, RoundedCorner.BottomLeft);
//		AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(40, 0), new Vector2(40, 0) + Style.WindowMinSize, Style.WindowRounding, RoundedCorner.TopLeft);
//		AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(0, 40), new Vector2(0, 40) + Style.WindowMinSize, Style.WindowRounding, RoundedCorner.TopRight);
//		AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(40, 40), new Vector2(40, 40) + Style.WindowMinSize, Style.WindowRounding, RoundedCorner.BottomRight);
//
//		AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(80, 0), new Vector2(80, 0) + Style.WindowMinSize, Style.WindowRounding, RoundedCorner.BottomRight | RoundedCorner.BottomLeft);
//		AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(80, 40), new Vector2(80, 40) + Style.WindowMinSize, Style.WindowRounding, RoundedCorner.TopLeft | RoundedCorner.TopRight);
//		AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(120, 0), new Vector2(120, 0) + Style.WindowMinSize, Style.WindowRounding, RoundedCorner.BottomRight | RoundedCorner.TopRight);
//		AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(120, 40), new Vector2(120, 40) + Style.WindowMinSize, Style.WindowRounding, RoundedCorner.TopLeft | RoundedCorner.BottomLeft);

//		AddFanFill(ColorPreset.Button, Vector2.left, 20f, 0, 19, 36);
//		AddText(ColorPreset.Text, Vector2.zero, "Hellow, world!\nFrom Dear ImGUI\nfor (int i = 0; i < count; i++) {\n\tconsole.log();\n}");

		ClearMesh();

		OnGui();

		ApplyMesh();

		Debug.Log("Rebuild complete at " + System.DateTime.Now);
	}

	string str = "";
	public void OnGui() {
		Window("ImGui Test");

		Text("ImGui says hello.");
		Text(string.Format("MousePos {0}\nIsPointerPressed {1}\nIsPointerPressing {2}", pointerPosition, isPointerPressed, isPointerHeld));

		Text("Same");
		SameLine();

		if (Button("Line", true)) {
			SameLine();
			Text("Pressed!");
			str += "#";
		}

		Text(str, true);

		EndWindow();
	}

	void Update() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		Debug.DrawRay(ray.origin, ray.direction * 10);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 100)) {
			Debug.DrawLine(hit.point, hit.point + Vector3.forward, Color.magenta);
		}


		time = Time.time;
		pointerPosition = hit.point;
		isPointerPressed = Input.GetMouseButtonDown(0);
		isPointerHeld = Input.GetMouseButton(0);

		if (isPointerPressed) {
			pointerPressedTime = time;
		}

		if (isPointerHeld) {
			if (!isPointerRepeating) {
				isPointerRepeating = time > pointerPressedTime + Style.KeyRepeatDelay;
			} else if (time > pointerPressedTime + Style.KeyRepeatRate) {
				pointerPressedTime = time;

				isPointerPressed = true;
			} else {
				isPointerPressed = false;
			}
		} else {
			isPointerRepeating = false;
		}

		ClearMesh();

		OnGui();

		ApplyMesh();		
	}

	void PushDrawingContext(Vector2 position, Vector2 size) {
		PushDrawingContext(new DrawingContext(position, size));
	}

	void PushDrawingContext(DrawingContext c) {
		AddRectFill(ColorPreset.Debug, c.root, c.span);
		drawingContextStack.Push(c);
	}

	void PopDrawingContext() {
		drawingContextStack.Pop();
	}

	DrawingContext GetCurrentDrawingContext() {
		if (drawingContextStack.Count < 1) return null;
		return drawingContextStack.Peek();
	}

	public bool HasSize(string name) {
		return sizeDict.ContainsKey(name);
	}

	public Vector2 GetSize(string name) {
		return sizeDict[name];
	}

	public void SetSize(string name, Vector2 size) {
		sizeDict[name] = size;
	}

	public void Window(string title = "Debug") {
		PushDrawingContext(Vector2.zero, HasSize(title) ? GetSize(title) : new Vector2(200, 200));

		var c = GetCurrentDrawingContext();

		AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(c.left, c.top - Style.TitleHeight), c.span, Style.WindowRounding, RoundedCorner.Bottom);
		AddRoundedRectFill(ColorPreset.TitleBg, c.root, c.FromTop(Style.TitleHeight), Style.WindowRounding, RoundedCorner.Top);

		AddText(ColorPreset.Text, c.FromRoot(Style.FramePadding), title, Style.FontSize);

		PushDrawingContext(c.Pad(Style.WindowPadding.x, Style.TitleHeight + Style.WindowPadding.y, Style.WindowPadding.x, Style.WindowPadding.y));
	}

	public void EndWindow() {
		PopDrawingContext();
		PopDrawingContext();

		Assert.Zero(drawingContextStack.Count);
	}

	public void Text(object o, bool wrap = false) {
		string text = o.ToString();

		var c = GetCurrentDrawingContext();
		c.NextItem(AddText(ColorPreset.Text, c.cursor, text, Style.FontSize, wrap));
	}

	public bool Button(string text, bool repeat = false) {
		var c = GetCurrentDrawingContext();

		var newCurosr = c.FromCursor(CalcTextSize(text, Style.FontSize) + Style.FramePadding * 2);

		bool hovered, pressed, held;
		pressed = ButtonBehaviour(c.cursor, newCurosr, out hovered, out held, repeat);

		var bg = ColorPreset.Button;
		if (pressed) bg = ColorPreset.ButtonActive;
		else if (hovered) bg = ColorPreset.ButtonHovered;

		AddRectFill(bg, c.cursor, newCurosr);
		AddText(ColorPreset.Text, c.FromCursor(Style.FramePadding), text, Style.FontSize);

		c.NextItem(newCurosr);

		return pressed;
	}
		
	public bool IsInDrawingContext(Vector2 a, Vector2 b, Vector2 v) {
		float xMin = Mathf.Min(a.x, b.x), xMax = Mathf.Max(a.x, b.x);
		float yMin = Mathf.Min(a.y, b.y), yMax = Mathf.Max(a.y, b.y);

		return xMin - Style.TouchExtraPadding < v.x && v.x < xMax + Style.TouchExtraPadding && yMin - Style.TouchExtraPadding < v.y && v.y < yMax + Style.TouchExtraPadding;
	}

	public bool ButtonBehaviour(Vector2 a, Vector2 b, out bool hovered, out bool held, bool repeat = false) {
		bool pressed = false;

		hovered = false;
		held = false;

		if (IsInDrawingContext(a, b, pointerPosition)) {
			hovered = true;

			pressed = isPointerPressed;
			if (!repeat) pressed = pressed && !isPointerRepeating;

			held = isPointerHeld;
		}

		return pressed;
	}

	public void SameLine() {
		GetCurrentDrawingContext().BackCursor();
	}

	bool ShouldClip(params Vector2[] vs) {
		var c = GetCurrentDrawingContext();

		if (c == null) return true;

		foreach (var v in vs) {
			if (!c.Contains(v)) return true;
		}

		return false;
	}

	public Vector2 CalcTextSize(string text, float size) {
		float scale = size / font.fontSize;

		text = text.Replace("\t", "    ");

		float x = 0, y = 0, xMax = x;
		foreach (var c in text) {
			uint uc = (uint)c;

			if (c == '\n') {
				x = 0;
				y -= size;
			} else if (font.HasGlyph(uc)) {
				UiFont.Glyph g = font.GetGlyph(uc);

				x += (g.xAdvance + font.spacingHoriz) * scale;

				if (x > xMax) xMax = x;
			}
		}

		return new Vector2(xMax - font.spacingHoriz * scale, -(y - size));
	}

	public Vector2 AddText(Color color, Vector2 pos, string text, float size, bool wrap = false) {
		var context = GetCurrentDrawingContext();

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
				UiFont.Glyph g = font.GetGlyph(uc);

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
				if (wrap && x > context.right) {  // Wrap text
					x = pos.x;
					y -= size;
				}

				if (x > xMax) xMax = x;
			}
		}

		AddRectFill(ColorPreset.Debug, pos, new Vector2(xMax - font.spacingHoriz * scale, y - size));
		return new Vector2(xMax - font.spacingHoriz * scale, y - size);
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

	[System.Flags]
	public enum RoundedCorner {
		TopLeft = 1 << 0,
		TopRight = 1 << 1,
		BottomLeft = 1 << 2,
		BottomRight = 1 << 3,

		All = TopLeft | TopRight | BottomLeft | BottomRight,

		Top = TopLeft | TopRight,
		Bottom = BottomLeft | BottomRight,

		Left = TopLeft | BottomLeft,
		Right = TopRight | BottomRight,
	}

	public void AddRoundedRectFill(Color color, Vector2 a, Vector2 b, float radius, RoundedCorner corners = RoundedCorner.All, int segments = 3) {
		bool roundTopLeft = ((corners & RoundedCorner.TopLeft) != 0);
		bool roundTopRight = ((corners & RoundedCorner.TopRight) != 0);
		bool roundBottomLeft = ((corners & RoundedCorner.BottomLeft) != 0);
		bool roundBottomRight = ((corners & RoundedCorner.BottomRight) != 0);

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

//		AddQuad(color / 2, v00, v01, v02, v03);
//		AddQuad(color / 2, v10, v11, v12, v13);

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
		Assert.AreEqual(0, triangleList.Count % 3);
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
		Assert.AreEqual(0, triangleList.Count % 3);
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
		Assert.AreEqual(vs.Length, uvs.Length);

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

	public void ApplyMesh() {
		Assert.AreEqual(vertexList.Count, uvList.Count);
		Assert.AreEqual(vertexList.Count, colorList.Count);

		Assert.AreEqual(0, triangleList.Count % 3);

		mesh.vertices = vertexList.ToArray();
		mesh.colors = colorList.ToArray();
		mesh.uv = uvList.ToArray();
		mesh.triangles = triangleList.ToArray();
	}

	public void OnDrawGizmosSelected() {
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
}
