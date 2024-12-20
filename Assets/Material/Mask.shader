Shader "Masked/Mask" {
    Properties
    {
        _StencilRef ("Stencil Ref", Int) = 1
    }

    SubShader {
        // 일반 지오메트리 이후, 마스크된 지오메트리와 투명한 오브젝트 이전에 렌더링
        Tags {"Queue" = "Geometry+10"}

        Stencil
        {
            Ref [_StencilRef]    // 스크립트에서 설정한 스텐실 값 사용
            Comp Always          // 항상 스텐실 값 기록
            Pass Replace         // 기존 스텐실 값 대체
        }

        // RGBA 채널에는 그리지 않고, 깊이 버퍼에만 기록
        ColorMask 0
        ZWrite On

        Pass {}
    }
}
