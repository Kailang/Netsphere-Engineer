namespace ImGui {
	[System.Flags]
	public enum RectCorner {
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
}

