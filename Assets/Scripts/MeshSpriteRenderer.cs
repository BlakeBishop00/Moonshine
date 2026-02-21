using UnityEngine;

public class MeshSpriteRenderer : MonoBehaviour
{
    public Material sharedMaterial;

    public MeshRenderer mr;
    static MaterialPropertyBlock mpb;

    void Awake()
    {
        if(mr == null)
        {
            mr = GetComponent<MeshRenderer>();
        }

        mr.sharedMaterial = sharedMaterial;

        if (mpb == null)
            mpb = new MaterialPropertyBlock();
    }

    public void ApplySprite(Sprite sprite, bool flipX)
    {
        if (sprite == null) return;

        // Size quad to sprite (PPU correct)
        float w = sprite.rect.width / sprite.pixelsPerUnit;
        float h = sprite.rect.height / sprite.pixelsPerUnit;
        mr.transform.localScale = new Vector3(
            flipX ? -w : w,
            h,
            1f
        );

        // Assign texture per instance
        mr.GetPropertyBlock(mpb);
        mpb.SetTexture("_MainTex", sprite.texture);
        mr.SetPropertyBlock(mpb);
    }
}
