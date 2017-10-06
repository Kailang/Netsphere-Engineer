using UnityEngine;

namespace ImGui {
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
	public class DrawContext {
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
		Vector2 cursorPrev_;
		float lineHeightPrev_;

		public Vector2 contentSize { get; private set; }
		public Vector2 cursor { get; private set; }

		public float padLeft { get; private set; }
		public float padTop { get; private set; }
		public float padRight { get; private set; }
		public float padBottom { get; private set; }

		public DrawContext(Vector2 root, Vector2 extent) {
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
			contentSize = new Vector2(Mathf.Max(contentSize.x, pos.x - root.x), Mathf.Max(contentSize.y, root.y - pos.y));

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

		public DrawContext Pad(float x, float y) {
			return Pad(x, y, x, y);
		}

		public DrawContext Pad(float left, float top, float right, float bottom) {
			padLeft = left;
			padTop = top;
			padRight = right;
			padBottom = bottom;

			var r = new Vector2(root.x + left, root.y - top);
			var s = new Vector2(width - left - right, height + top + bottom);

			if (s.x <= 0 || s.y >= 0) return new DrawContext(r, Vector2.zero);
			return new DrawContext(r, s);
		}

		public Vector2 CalcSize(Vector2 contentSize) {
			return new Vector2(contentSize.x + padLeft + padRight, contentSize.y + padTop + padBottom);
		}
	}
}

