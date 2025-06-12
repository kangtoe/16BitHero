= README

Autotilesets (both low and high) are designed according to
https://github.com/godotengine/godot-docs/issues/3316 (3x3 minimal).

The 16x16 atlas is designes so there is little setup needed in most
engines. Each tile is exactly 16x16 pixels and they are supposed to
be placed on 16x16 grid, no layers needed.

The 16x32 (high) set there most of the tiles are set with the Y offsed
of 8, some of them -2 (you'll figure which one is which, you can use
different values is the difference is 10). Some tiles are meant to be
drawn on a layer above (so they modify the one below). The tile size is
bigger than the grid that they are supposed to be placed on (16x16).

자동 타일셋(낮음 및 높음 모두)은
https://github.com/godotengine/godot-docs/issues/3316(최소 3x3)에 따라 설계되었습니다.

16x16 아틀라스는 대부분의 엔진에서 설정이 거의 필요하지 않도록 설계되었습니다.
각 타일은 정확히 16x16 픽셀이며, 16x16 그리드에 배치되어야 하며, 레이어는 필요하지 않습니다.

16x32(높음) 세트의 경우, 대부분의 타일은 Y축 오프셋이 8로 설정되어 있으며, 일부 타일은 -2로 설정되어 있습니다(어떤 타일이 어떤 타일인지는 나중에 알 수 있습니다.
다른 값을 사용하면 10만큼 차이가 날 수 있습니다). 일부 타일은
위 레이어에 그려지도록 설계되어 있으므로 아래 레이어를 수정합니다. 타일 크기는 배치될 그리드(16x16)보다 큽니다.

