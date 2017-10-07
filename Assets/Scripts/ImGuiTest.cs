using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using ImGui;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ImGuiTest : MonoBehaviour {
	public Mesh mesh;

	DrawList list;
	BitmapFont font;

	public Vector2 pointerPosition;
	public bool isPointerPressed, isPointerHeld, isPointerRepeating;
	public float pointerPressedTime, time;

	Dictionary<string, Vector2> sizeDict = new Dictionary<string, Vector2>();

	readonly Stack<DrawContext> contextStack = new Stack<DrawContext>();
	DrawContext context;

	float itemWidth;

	int selectionStart, selectionEnd;
	string stringBuffer;

	public void OnValidate() {
		using (var stream = new System.IO.MemoryStream(Resources.Load<TextAsset>("ProggyCleanFont").bytes)) {
			font = new BitmapFont(stream);
		}

		list = new DrawList();
		GetComponent<MeshFilter>().mesh = mesh = list.mesh;

		OnGui();

		list.ClearMesh();

//		list.PrimitiveTest(font);

		OnGui();

		list.ApplyMesh();

		Debug.Log("Rebuild complete at " + System.DateTime.Now);
	}

	string stringVal = "abc";
	bool boolVal = true;
	int intVal = 1;

	public void OnGui() {
		Window("ImGui Test");

		itemWidth = 0.65f;

		Text("ImGui says hello.");
		Text(string.Format("MousePos {0}\nIsPointerPressed {1}\nIsPointerHeld {2}", pointerPosition, isPointerPressed, isPointerHeld));

		Text("Same");
		SameLine();

		if (Button("Line", true)) {
			SameLine();
			Text("Pressed!");
			stringVal += "#";
		}

		Separator();
		Bullet();
		Text(stringVal, true);

		Separator();
		Bullet();
		Bullet();
		Bullet();
		Text("Hello!");

		Separator();

		Checkbox("I'm a checkbox", ref boolVal, true);

		Separator();

		Checkbox("I'm anthor checkbox", ref boolVal, true);
		SameLine();
		Checkbox("Follows", ref boolVal, true);

		Separator();
		LabeledText("Text", stringVal);
		Separator();
		LabeledText("Text", stringVal);

		RadioButton("I1", ref intVal, 0);
		SameLine();
		RadioButton("R2", ref intVal, 1);
		SameLine();
		RadioButton("R3", ref intVal, 2);
		SameLine();
		RadioButton("R4", ref intVal, 3);

		EndWindow();
	}

	void Update() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, 100)) {
			Debug.DrawLine(hit.point, hit.point + Vector3.forward, Color.magenta);
		}

		time = Time.time;
		pointerPosition = hit.point;
		isPointerPressed = Input.GetMouseButtonDown(0);
		isPointerHeld = Input.GetMouseButton(0);

		ProcessInput();

		list.ClearMesh();

		OnGui();

		list.ApplyMesh();		
	}

	public void ProcessInput() {
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
	}

	public float CalcItemWidth() {
		float width = context.width;
		float res = itemWidth;

		if (-1 < itemWidth && itemWidth < 1) {
			res *= width;
		}

		return res < 0 ? width + res : res;
	}

	public void PushContext(Vector2 position, Vector2 size) {
		PushContext(new DrawContext(position, size));
	}

	public void PushContext(DrawContext c) {
		context = c;
		list.SetContext(context);
		contextStack.Push(c);
	}

	public DrawContext PopContext() {
		context = contextStack.Pop();
		list.SetContext(context);
		return context;
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
		PushContext(Vector2.zero, HasSize("Debug") ? GetSize("Debug") : Style.WindowMinSize);

		list.AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(context.left, context.top - Style.TitleHeight), context.span, Style.WindowRounding, RectCorner.Bottom);
		list.AddRoundedRectFill(ColorPreset.TitleBg, context.root, context.FromMax(new Vector2(0, Style.TitleHeight)), Style.WindowRounding, RectCorner.Top);

		list.AddText(font, ColorPreset.Text, context.FromRoot(Style.FramePadding), title, Style.FontSize);

		PushContext(context.Pad(Style.WindowPadding.x, Style.TitleHeight + Style.WindowPadding.y, Style.WindowPadding.x, Style.WindowPadding.y));
	}

	public void EndWindow() {
		var size = PopContext().contentSize;
		SetSize("Debug", PopContext().CalcSize(size, true, true));

		Assert.Zero(contextStack.Count);
	}

	public void Bullet() {
		float radius = 0.5f + (int)(Style.FontSize * 0.2f);
		list.AddCircleFill(ColorPreset.Text, context.FromCursor(Style.FontSquare * 0.5f), radius, 6);
		context.NextItem(context.FromCursor(Style.FontSquare), Style.ItemInnerSpacing, false);
	}

	public void Separator() {
		float y = context.cursor.y + Style.FramePadding.y / 2;
		list.AddLine(ColorPreset.Border, new Vector2(context.root.x, y), new Vector2(context.max.x, y), 1);
		context.ShiftCursor(new Vector2(0, -1));
	}

	public void Text(string text, bool wrap = false) {
		context.NextItem(list.AddText(font, ColorPreset.Text, context.cursor, text, Style.FontSize, wrap), Style.ItemSpacing);
	}

	public void LabeledText(string label, string text) {
		float right = context.cursor.x + CalcItemWidth();
		var cursor = 
			list.AddText(font, ColorPreset.Text, 
				context.cursor, text, Style.FontSize, true, right);

		context.NextItem(new Vector2(right, cursor.y), Style.ItemSpacing);
		context.BackCursor(Style.ItemSpacing);
		context.NextItem(list.AddText(font, ColorPreset.Text, context.cursor, label, Style.FontSize), Style.ItemSpacing);
	}

	public bool Button(string text, bool repeat = false) {
		var cursor = context.FromCursor(list.CalcTextSize(font, text, Style.FontSize) + Style.FramePadding * 2);

		bool hovered, pressed, held;
		pressed = ButtonBehaviour(context.cursor, cursor, out hovered, out held, repeat);

		list.AddRoundedRectFill(
			SelectColor(pressed, hovered, ColorPreset.ButtonActive, ColorPreset.ButtonHovered, ColorPreset.Button), 
			context.cursor, cursor, Style.FrameRounding);
		list.AddText(font, ColorPreset.Text, context.FromCursor(Style.FramePadding), text, Style.FontSize);

		context.NextItem(cursor, Style.ItemSpacing);

		return pressed;
	}

	public bool Checkbox(string label, ref bool value, bool repeat = false) {
		var size = new Vector2(Style.FontSize + Style.FramePadding.y * 2, Style.FontSize + Style.FramePadding.y * 2);

		bool hovered, pressed, held;
		if (pressed = ButtonBehaviour(context.cursor, context.FromCursor(size), out hovered, out held, repeat)) {
			value = !value;
		}

		float pad = Mathf.Max(1.0f, (int)(Style.FontSize / 6));

		list.AddRoundedRectFill(
			SelectColor(pressed, hovered, ColorPreset.FrameBgActive, ColorPreset.FrameBgHovered, ColorPreset.FrameBg),
			context.cursor, context.FromCursor(size), Style.FrameRounding);

		if (value) list.AddRoundedRectFill(
				ColorPreset.CheckMark,
				context.FromCursor(new Vector2(pad, pad)), 
				context.FromCursor(size - new Vector2(pad, pad)), Style.FrameRounding);


		context.NextItem(context.FromCursor(size), Style.ItemInnerSpacing, false);
		context.NextItem(list.AddText(font, ColorPreset.Text, context.cursor + new Vector2(0, -Style.FramePadding.y), label, Style.FontSize), Style.ItemSpacing);
		
		return true;
	}

	public bool RadioButton(string label, ref int value, int me) {
		var size = new Vector2(Style.FontSize + Style.FramePadding.y * 2, Style.FontSize + Style.FramePadding.y * 2);
		var radius = size / 2;

		bool hovered, pressed, held;
		if (pressed = ButtonBehaviour(context.cursor, context.FromCursor(size), out hovered, out held)) {
			value = me;
		}

		float pad = Mathf.Max(1.0f, (int)(Style.FontSize / 6));

		list.AddCircleFill(
			SelectColor(pressed, hovered, ColorPreset.FrameBgActive, ColorPreset.FrameBgHovered, ColorPreset.FrameBg),
			context.FromCursor(radius), radius.x);

		if (value == me) list.AddCircleFill(
				ColorPreset.CheckMark,
				context.FromCursor(radius), radius.x - pad);


		context.NextItem(context.FromCursor(size), Style.ItemInnerSpacing, false);
		context.NextItem(list.AddText(font, ColorPreset.Text, context.cursor + new Vector2(0, -Style.FramePadding.y), label, Style.FontSize), Style.ItemSpacing);

		return true;
	}
		
	public bool IsPointerInside(Vector2 a, Vector2 b) {
		float xMin = Mathf.Min(a.x, b.x), xMax = Mathf.Max(a.x, b.x);
		float yMin = Mathf.Min(a.y, b.y), yMax = Mathf.Max(a.y, b.y);

		return xMin - Style.TouchExtraPadding < pointerPosition.x && pointerPosition.x < xMax + Style.TouchExtraPadding && yMin - Style.TouchExtraPadding < pointerPosition.y && pointerPosition.y < yMax + Style.TouchExtraPadding;
	}

	public bool ButtonBehaviour(Vector2 a, Vector2 b, out bool hovered, out bool held, bool repeat = false) {
		bool pressed = false;

		hovered = false;
		held = false;

		if (IsPointerInside(a, b)) {
			hovered = true;

			pressed = isPointerPressed;
			if (!repeat) pressed = pressed && !isPointerRepeating;

			held = isPointerHeld;
		}

		return pressed;
	}

	public void SameLine() {
		context.BackCursor(Style.ItemSpacing);
	}

	Color SelectColor(bool active, bool hovered, Color activeColor, Color hoveredColor, Color color) {
		if (active) return activeColor;
		if (hovered) return hoveredColor;
		return color;
	}

	public void OnDrawGizmos() {
		list.DrawGizmos();
	}
}
