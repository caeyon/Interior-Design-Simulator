using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public enum Direction
    {
        DirX,
        DirY,
        DirZ,
    }

    public int Row;

    public int Col;

    public Color LineColor;

    public Direction MyDirection;

    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }

    private void OnDrawGizmos()
    {
        // Draw Scene
        CreateLineMaterial();
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        DrawGrid(Row, Col, Direction.DirX);
        GL.PopMatrix();
    }

    private void OnRenderObject()
    {
        if (Camera.current.name != "Main Camera")
            return;

        CreateLineMaterial();
        lineMaterial.SetPass(0);

        GL.PushMatrix();

        if (MyDirection == Direction.DirX)
        {
            DrawGrid(Row, Col, Direction.DirX);
        }
        else if (MyDirection == Direction.DirY)
        {
            DrawGrid(Row, Col, Direction.DirY);
        }
        else if (MyDirection == Direction.DirZ)
        {
            DrawGrid(Row, Col, Direction.DirZ);
        }
        GL.PopMatrix();
    }

    private Color GetAlphaDistance(int i)
    {
        double alpha = 0.1f;
       /* if (i < 0)
            i = i * -1;

        if (i == 0)
            alpha = 1f;

        double temp = (double)i / (double)Row;
        alpha = temp * 100f;
        alpha = 1 - (alpha / 100);

        if (alpha > 0.8)
            alpha = 0.8f;*/

        return new Color(LineColor.r, LineColor.g, LineColor.b, (float)alpha);
    }

    void DrawGrid(int row, int col, Direction direction)
    {
        GL.Begin(GL.LINES);
        GL.Color(LineColor);

        if (direction == Direction.DirX)
        {
            // row
            for (int i = -row; i <= row; i++)
            {
                GL.Color(GetAlphaDistance(i));
                GL.Vertex3((float)-row, 0, (float)i);
                GL.Vertex3((float)row, 0, (float)i);
            }

            // col
            for (int i = -col; i <= col; i++)
            {
                GL.Color(GetAlphaDistance(i));
                GL.Vertex3((float)i, 0, (float)-col);
                GL.Vertex3((float)i, 0, (float)col);
            }
        }
        else if (direction == Direction.DirY)
        {
            // row
            for (int i = -row; i <= row; i++)
            {
                GL.Color(GetAlphaDistance(i));
                GL.Vertex3((float)-row, (float)i, 0);
                GL.Vertex3((float)row, (float)i, 0);
            }

            // col
            for (int i = -col; i <= col; i++)
            {
                GL.Color(GetAlphaDistance(i));
                GL.Vertex3((float)i, (float)-col, 0);
                GL.Vertex3((float)i, (float)col, 0);
            }
        }
        else if (direction == Direction.DirZ)
        {
            // row
            for (int i = -row; i <= row; i++)
            {
                GL.Color(GetAlphaDistance(i));
                GL.Vertex3(0, (float)-row, (float)i);
                GL.Vertex3(0, (float)row, (float)i);
            }

            // col
            for (int i = -col; i <= col; i++)
            {
                GL.Color(GetAlphaDistance(i));
                GL.Vertex3(0, (float)i, (float)-col);
                GL.Vertex3(0, (float)i, (float)col);
            }
        }
        GL.End();
    }
}
