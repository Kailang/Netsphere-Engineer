using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using ImGui;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ImGuiTest : MonoBehaviour {
	const float kPixelsPerUnit = 10;

	const int kNullId = -1;
	public Vector2 pointerPosition;
	public bool isPointerPressed, isPointerHeld, isPointerRepeating;
	public float pointerPressedTime, time;

	readonly Dictionary<string, Vector2> sizeDict = new Dictionary<string, Vector2>();

	readonly Stack<DrawContext> drawContextStack = new Stack<DrawContext>();
	DrawContext context;

	DrawList list;
	public Mesh mesh;

	BitmapFont font;

	public void OnValidate() {
		using (var stream = new System.IO.MemoryStream(Resources.Load<TextAsset>("ProggyCleanFont").bytes)) {
			font = new BitmapFont(stream);
		}

		list = new DrawList(GetComponent<MeshFilter>().mesh = mesh = new Mesh());

		list.ClearMesh();

		OnGui();

		list.ApplyMesh();

		Debug.Log("Rebuild complete at " + System.DateTime.Now);
	}

	string str = "";
	public void OnGui() {
		Window("ImGui Test");

		Text("ImGui says hello.");
		Text(string.Format("MousePos {0}\nIsPointerPressed {1}\nIsPointerHeld {2}", pointerPosition, isPointerPressed, isPointerHeld));

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

		list.ClearMesh();

		OnGui();

		list.ApplyMesh();		
	}

	void PushDrawContext(Vector2 position, Vector2 size) {
		PushDrawContext(new DrawContext(position, size));
	}

	void PushDrawContext(DrawContext c) {
//		AddRectFill(ColorPreset.Debug, c.root, c.span);
		context = c;
		list.SetDrawingContext(c);
		drawContextStack.Push(c);
	}

	DrawContext PopDrawContext() {
		context = drawContextStack.Pop();
		list.SetDrawingContext(context);
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
		PushDrawContext(Vector2.zero, HasSize("Debug") ? GetSize("Debug") : Style.WindowMinSize);

		list.AddRoundedRectFill(ColorPreset.WindowBg, new Vector2(context.left, context.top - Style.TitleHeight), context.span, Style.WindowRounding, RectCorner.Bottom);
		list.AddRoundedRectFill(ColorPreset.TitleBg, context.root, context.FromTop(Style.TitleHeight), Style.WindowRounding, RectCorner.Top);

		list.AddText(font, ColorPreset.Text, context.FromRoot(Style.FramePadding), title, Style.FontSize);

		PushDrawContext(context.Pad(Style.WindowPadding.x, Style.TitleHeight + Style.WindowPadding.y, Style.WindowPadding.x, Style.WindowPadding.y));
	}

	public void EndWindow() {
		var size = PopDrawContext().contentSize;
		SetSize("Debug", PopDrawContext().CalcSize(size));

		Assert.Zero(drawContextStack.Count);
	}

	public void Text(object o, bool wrap = false) {
		string text = o.ToString();

		context.NextItem(list.AddText(font, ColorPreset.Text, context.cursor, text, Style.FontSize, wrap));
	}

	public bool Button(string text, bool repeat = false) {
		var newCurosr = context.FromCursor(list.CalcTextSize(font, text, Style.FontSize) + Style.FramePadding * 2);

		bool hovered, pressed, held;
		pressed = ButtonBehaviour(context.cursor, newCurosr, out hovered, out held, repeat);

		var bg = ColorPreset.Button;
		if (pressed) bg = ColorPreset.ButtonActive;
		else if (hovered) bg = ColorPreset.ButtonHovered;

		list.AddRectFill(bg, context.cursor, newCurosr);
		list.AddText(font, ColorPreset.Text, context.FromCursor(Style.FramePadding), text, Style.FontSize);

		context.NextItem(newCurosr);

		return pressed;
	}
		
	public bool IsInDrawContext(Vector2 a, Vector2 b, Vector2 v) {
		float xMin = Mathf.Min(a.x, b.x), xMax = Mathf.Max(a.x, b.x);
		float yMin = Mathf.Min(a.y, b.y), yMax = Mathf.Max(a.y, b.y);

		return xMin - Style.TouchExtraPadding < v.x && v.x < xMax + Style.TouchExtraPadding && yMin - Style.TouchExtraPadding < v.y && v.y < yMax + Style.TouchExtraPadding;
	}

	public bool ButtonBehaviour(Vector2 a, Vector2 b, out bool hovered, out bool held, bool repeat = false) {
		bool pressed = false;

		hovered = false;
		held = false;

		if (IsInDrawContext(a, b, pointerPosition)) {
			hovered = true;

			pressed = isPointerPressed;
			if (!repeat) pressed = pressed && !isPointerRepeating;

			held = isPointerHeld;
		}

		return pressed;
	}

	public void SameLine() {
		context.BackCursor();
	}

	public void OnDrawGizmos() {
		list.DrawGizmos();
	}
}
