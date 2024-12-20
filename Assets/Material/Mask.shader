Shader "Masked/Mask" {
    Properties
    {
        _StencilRef ("Stencil Ref", Int) = 1
    }

    SubShader {
        // �Ϲ� ������Ʈ�� ����, ����ũ�� ������Ʈ���� ������ ������Ʈ ������ ������
        Tags {"Queue" = "Geometry+10"}

        Stencil
        {
            Ref [_StencilRef]    // ��ũ��Ʈ���� ������ ���ٽ� �� ���
            Comp Always          // �׻� ���ٽ� �� ���
            Pass Replace         // ���� ���ٽ� �� ��ü
        }

        // RGBA ä�ο��� �׸��� �ʰ�, ���� ���ۿ��� ���
        ColorMask 0
        ZWrite On

        Pass {}
    }
}
