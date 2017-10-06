using UnityEngine;

namespace ImGui {
	public static class Style {
		public static Vector2 WindowPadding = new Vector2(8, 8);
		// Padding within a window
		public static Vector2 WindowMinSize = new Vector2(32, 32);
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
}

