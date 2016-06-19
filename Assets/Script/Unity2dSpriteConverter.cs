using UnityEngine;
using System.Collections;

//UiSpriteをUnity2D用のspriteに変換する
public class Unity2dSpriteConverter : MonoBehaviour {

    [SerializeField]
	private UISprite[] originals;    //!<変換対象の画像をセットする(現状は背景画像のみ)


	// Use this for initialization
	void Start () {
		ConverterMain();
	}

    


    void ConverterMain()
    {
        foreach (UISprite original in originals)
        {
            //変換する
            Unity2dSpriteConverter.ConverterSpriteRenderer(original);

        }
    }




    /**
     * @brief UispriteからUnity2d(SpriteRenderer)に変換する
     * @param Uisprite 
     */
    public static void ConverterSpriteRenderer(UISprite original)
    {
        if (original == null) return;
        if (original.atlas == null) return;
        Texture2D texture2D = original.atlas.texture as Texture2D;
        UISpriteData data = original.GetAtlasSprite();

        //テクスチャを呼び出す
        var texRect = new Rect(data.x, texture2D.height - data.y - data.height, data.width, data.height);

        //pivotの設定
        //デフォルトを中央基準とする
        Vector2 pivot = new Vector2(0.5f, 0.5f);

        switch (original.pivot)
        {
            case UIWidget.Pivot.TopLeft:
                //左上基準
                pivot = new Vector2(0, 1.0f);
                break;

            case UIWidget.Pivot.Bottom:
                //下中央基準
                pivot = new Vector2(0.5f, 0f);

                break;

            default:

                break;

        }
    



        var sprite = Sprite.Create(texture2D, texRect, pivot, 1, 1, SpriteMeshType.FullRect);
        SpriteRenderer r = original.gameObject.GetComponent<SpriteRenderer>();

        if (r == null)
        {
            r = original.gameObject.AddComponent<SpriteRenderer>();
        }

        original.enabled = false;
        r.sprite = sprite;
        r.enabled = true;



		//unity2d spriteのマテリアルをオリジナル(NGUI)のものに変更する
        if(original.atlas != null)
        {
            r.GetComponent<Renderer>().material = original.atlas.spriteMaterial;
        }

        Vector3 vec;
        float originalCenterPos, convertCenterPos, diff;
        //---------------------------------------------------------
		//Y方向 padding分位置補正(中央基準、下基準のみ対応、他の基準が来たらそのときに対応が必要) 
        //---------------------------------------------------------
        if (original.pivot == UIWidget.Pivot.Center)
        {

            //元テクスチャの画像側の中心位置を取得し、下側の余白を取得する
            originalCenterPos = (data.height / 2) + data.paddingBottom;

            //余白を抜いた画像のメタデータ取得
            convertCenterPos = original.height / 2;

            //差を計算する
            diff = convertCenterPos - originalCenterPos;

            //scall分かける
            diff *= original.gameObject.transform.localScale.y;

            vec = original.gameObject.transform.localPosition;
            original.gameObject.transform.localPosition = new Vector3(vec.x, vec.y - diff, vec.z);
        }
        else if (original.pivot == UIWidget.Pivot.Bottom)
        {
            //差を計算する
            diff = data.paddingBottom;

            //scall分かける
            diff *= original.gameObject.transform.localScale.y;

            vec = original.gameObject.transform.localPosition;
            original.gameObject.transform.localPosition = new Vector3(vec.x, vec.y + diff, vec.z);
          
        }

        //float originalCenterPos, convertCenterPos, diff;
        //Vector3 vec;
        //---------------------------------------------------------
		//X方向 padding分位置補正(中央基準、下基準のみ対応、他の基準が来たらそのときに対応が必要) 
        //---------------------------------------------------------
        if (original.pivot == UIWidget.Pivot.Center || original.pivot == UIWidget.Pivot.Bottom)
        {

            //元テクスチャの画像側の中心位置を取得し、下側の余白を取得する
            originalCenterPos = (data.width / 2) + data.paddingLeft;

            //余白を抜いた画像のメタデータ取得
            convertCenterPos = original.width / 2;

            //差を計算する
            diff = convertCenterPos - originalCenterPos;

            //scall分かける
            diff *= original.gameObject.transform.localScale.x;

            vec = original.gameObject.transform.localPosition;
            original.gameObject.transform.localPosition = new Vector3(vec.x - diff, vec.y, vec.z);
        }
        
    }
}
