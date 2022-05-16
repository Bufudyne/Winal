using TMPro;
using UnityEngine;

public class CharacterWobble : MonoBehaviour
{
    public float xOffset = 3.3f;
    public float yOffset = 2.5f;
    private TextMeshProUGUI textMesh;

    private Mesh mesh;

    private Vector3[] vertices;

    // Start is called before the first frame update
    private void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    private void Update()
    {
        textMesh.ForceMeshUpdate();
        mesh = textMesh.mesh;
        vertices = mesh.vertices;

        for (var i = 0; i < textMesh.textInfo.characterCount; i++)
        {
            var c = textMesh.textInfo.characterInfo[i];

            var index = c.vertexIndex;

            Vector3 offset = Wobble(Time.time + i);
            vertices[index] += offset;
            vertices[index + 1] += offset;
            vertices[index + 2] += offset;
            vertices[index + 3] += offset;
        }

        mesh.vertices = vertices;
        textMesh.canvasRenderer.SetMesh(mesh);
    }

    private Vector2 Wobble(float time)
    {
        return new Vector2(Mathf.Sin(time * xOffset), Mathf.Cos(time * yOffset));
    }
}