using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bina : MonoBehaviour
{
    public GameObject uretilen_birim;
    public GameObject cikis_noktasi;
    public GameObject varis_noktasi;

    public GameObject normal_model;
    public GameObject insaat_modeli;

    float zaman;

    public float gerekli_insaat_suresi;
    public float insaat_suresi;
    bool insaat_bitti;

    private AnaOyun anaOyun;

    public void InsaataBasla()
    {
        insaat_modeli.SetActive(true);
        normal_model.SetActive(false);
        insaat_bitti = false;
        insaat_suresi = 0;

        SetLayerRecursively(gameObject, 11);
    }

    public bool InsaataKatkidaBulun(float katki)
    {
        insaat_suresi += katki;
        if(insaat_suresi >= gerekli_insaat_suresi)
        {
            insaat_bitti = true;
            insaat_modeli.SetActive(false);
            normal_model.SetActive(true);
            SetLayerRecursively(gameObject, 8);
            anaOyun.kaynak_birakma_binalari.Add(this);
            return true;
        }

        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        zaman = 5f;
        anaOyun = GameObject.Find("AnaOyun").GetComponent<AnaOyun>();
    }

    // Update is called once per frame
    void Update()
    {/*
        zaman -= Time.deltaTime;
        if(zaman <= 0)
        {
            GameObject yeniBirim = Instantiate(uretilen_birim, cikis_noktasi.transform.position, Quaternion.identity);
            Birim b = yeniBirim.GetComponent<Birim>();
            b.AgentBelirle();
            b.Yuru(varis_noktasi.transform.position);
            zaman = 5f;
        }*/

    }

    public void VarisSec(Vector3 konum)
    {
        varis_noktasi.transform.SetPositionAndRotation(konum, Quaternion.identity);
    }

    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        if (null == obj)
        {
            return;
        }

        obj.layer = newLayer;

        foreach (Transform child in obj.transform)
        {
            if (null == child)
            {
                continue;
            }
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
