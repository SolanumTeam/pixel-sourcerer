shader_type canvas_item;
uniform sampler2D look_up: filter_nearest;

void fragment() {
	vec4 source = texture(TEXTURE, vec2(UV.x, UV.y));
	vec4 look_up_color = texture(look_up, vec2(source.r, source.g));
	COLOR = vec4(vec3(look_up_color.rgb), source.a);
}
