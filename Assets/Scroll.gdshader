shader_type canvas_item;

uniform bool stop;

void vertex() {
	if (!stop) {
		UV.y -= TIME;
	}
}
