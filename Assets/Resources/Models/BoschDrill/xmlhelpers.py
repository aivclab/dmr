mesh_material_pairs = [("second_plastic_ring", "wire_008008136"),
("front_plastic_ring", "wire_008008136"),
("front_metal_hooks", "wire_057008136"),
("flat_head_bit", "wire_000000000"),
("front_metal_ring", "wire_008008136"),
("body", "wire_108008136"),
("back_rubber", "wire_108008136"),
("battery", "wire_108008136"),
("slide_button", "wire_028149177"),
("silver_back", "wire_108008136"),
("main_button", "wire_148177027"),
("battery_indicator_glass", "wire_108008136"),
("indicator", "wire_028089177"),
("upper_button", "wire_108008136"),
("lower_button", "wire_108008136"),
("logo", "wire_028089177"),
("podlozka__battery_indicator", "wire_108008136"),
("button", "wire_108008136"),
("button01", "wire_108008136"),
("button02", "wire_108008136"),
("button3", "wire_108008136"),
("battery_button", "wire_108008136"),
("logo", "wire_028089177"),
("podlozka__battery_indicator001", "black_back_plate")]

object_template = """	<TriangleObject>
		<Name>##MESHNAME##</Name>
		<MeshFile>##MESHNAME##.dae</MeshFile>
		<MaterialName>##MATERIALNAME##</MaterialName>
		<Matrix4x4 f00="1" f01="0" f02="0" f03="0" f10="0" f11="1" f12="0" f13="0" f20="0" f21="0" f22="1" f23="0" f30="0" f31="0" f32="0" f33="1" />
	</TriangleObject>"""

for o, m in mesh_material_pairs:
    xml = object_template.replace("##MESHNAME##", o)
    xml = xml.replace("##MATERIALNAME##", m)
    print(xml)
    
    
    
    