shader_type canvas_item;
uniform sampler2D noise: repeat_enable;
uniform vec4 main_color: source_color;
uniform vec4 second_color: source_color;
uniform vec2 scroll = vec2(-0.05, -0.05);

void fragment() {
	float intensity = smoothstep(0.5, 1, texture(noise, UV.xy + scroll * TIME).r);
	COLOR = vec4(mix(main_color, second_color, intensity));
}
