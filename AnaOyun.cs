using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.AI;

public enum Mod { Normal, BinaÜret };

public class AnaOyun : MonoBehaviour
{
    [Header("Arayüz Parçalarý")]
    public Text kaynak0;
    public Text kaynak1;

    public GameObject birimPaneli;

    public Text birim_isim;
    public Text birim_can;
    public Text birim_saldiri;
    public Text birim_savunma;
    public Text birim_buyu;

    public Image birim_resim;
    public Sprite[] birim_resim_katalogu;

    [Header("Diðer Deðiþkenler")]
    public float[] kaynaklar;

    public Material mat_normal;
    public Material mat_secili;
    public Material mat_toplama;

    public Material mat_binaolur;
    public Material mat_binaolmaz;

    private GameObject mevcut_secili;

    public List<Bina> kaynak_birakma_binalari;

    Mod mevcutMod = Mod.Normal;
    GameObject eldekiBina;

    public GameObject[] bina_katalogu;

    public void KaynakBirak(Birim b)
    {
        kaynaklar[b.toplanan_kaynak_cinsi] += b.eldeki_kaynak;
        b.eldeki_kaynak = 0;
        switch(b.toplanan_kaynak_cinsi)
        {
            case 0:
                kaynak0.text = "" + kaynaklar[0];
                break;
            case 1:
                kaynak1.text = "" + kaynaklar[1];
                break;
        }
    }

    public bool KaynakBirakacakNoktaBul(Birim b)
    {
        if(kaynak_birakma_binalari.Count == 0)
        {
            return false;
        }
        else
        {
            Vector3 birimKonumu = b.gameObject.transform.position;
            Vector3 enYakinKonum = kaynak_birakma_binalari[0].cikis_noktasi.transform.position;
            float enYakinMesafe = Vector3.Distance(birimKonumu,enYakinKonum);

            GameObject enYakinKonumObjesi = kaynak_birakma_binalari[0].cikis_noktasi;

            for (int i = 1; i < kaynak_birakma_binalari.Count; i++)
            {
                Vector3 simdikiKonum = kaynak_birakma_binalari[i].cikis_noktasi.transform.position;
                float simdikiMesafe = Vector3.Distance(birimKonumu, simdikiKonum);

                if(simdikiMesafe < enYakinMesafe)
                {
                    enYakinMesafe = simdikiMesafe;
                    enYakinKonum = simdikiKonum;
                    enYakinKonumObjesi = kaynak_birakma_binalari[i].cikis_noktasi;
                }
            }

            b.donus_noktasi = enYakinKonumObjesi;

            return true;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        kaynak0.text = "" + kaynaklar[0];
        kaynak1.text = "" + kaynaklar[1];
    }

    void BirimArayuzuGuncelleme(bool secildi)
    {
        if(secildi)
        {
            LayerMask lm = LayerMask.GetMask("Birim");

            if (lm == (lm | (1 << mevcut_secili.layer)))
            {
                // bu bir birimdir

                Birim b = mevcut_secili.GetComponentInParent<Birim>();
                if(b != null)
                {
                    birim_isim.text = b.isim;
                    birim_can.text = b.can + "/" + b.can_kapasite;
                    birim_saldiri.text = "" + b.saldiri;
                    birim_savunma.text = "" + b.savunma;
                    birim_buyu.text = "" + b.buyu;

                    birim_resim.sprite = birim_resim_katalogu[b.resim_indeksi];
                }
                
            }
            else
            {
                lm = LayerMask.GetMask("Bina");

                if (lm == (lm | (1 << mevcut_secili.layer)))
                {
                    // bu bir binadýr
                }
            }
                

            birimPaneli.SetActive(true);
        }
        else
        {
            birimPaneli.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if(mevcutMod == Mod.Normal)
        {
            // seçim yapma
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Birim", "Bina")))
                {
                    if (mevcut_secili != null)
                    {
                        MeshRenderer mr_eski = mevcut_secili.GetComponent<MeshRenderer>();
                        if (mr_eski != null)
                        {
                            mr_eski.material = mat_normal;
                        }
                    }

                    // bir þey seçtik
                    mevcut_secili = hit.transform.gameObject;
                    MeshRenderer mr = mevcut_secili.GetComponent<MeshRenderer>();
                    if (mr != null)
                    {
                        mr.material = mat_secili;
                    }

                    BirimArayuzuGuncelleme(true);

                }
                else
                {
                    // boþluða týkladýk
                    if (mevcut_secili != null)
                    {
                        MeshRenderer mr_eski = mevcut_secili.GetComponent<MeshRenderer>();
                        if (mr_eski != null)
                        {
                            mr_eski.material = mat_normal;
                        }
                        mevcut_secili = null;

                        BirimArayuzuGuncelleme(false);
                    }
                }
            }
            // emir verme
            else if (Input.GetMouseButtonDown(1))
            {
                if (mevcut_secili != null)
                {
                    LayerMask lm = LayerMask.GetMask("Birim");

                    if (lm == (lm | (1 << mevcut_secili.layer)))
                    {
                        // seçili olan þey bir birim

                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                        // kaynaða mý sað týkladýk?
                        if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Kaynak")))
                        {
                            Birim b = mevcut_secili.GetComponentInParent<Birim>();
                            if (b != null)
                            {
                                Kaynak k = hit.transform.gameObject.GetComponent<Kaynak>();
                                if (k != null)
                                {
                                    // b birimi k kaynaðýný toplamalý
                                    if (b.kaynak_toplayabilir)
                                    {
                                        b.KaynagaGit(k);
                                    }
                                    else
                                    {
                                        b.Yuru(hit.point);
                                    }
                                }
                            }
                        }
                        // inþaata mý sað týkladýk?
                        else if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Insaat"))) {
                            Birim b = mevcut_secili.GetComponentInParent<Birim>();
                            if(b!= null)
                            {
                                Bina bina = hit.transform.gameObject.GetComponentInParent<Bina>();
                                b.InsaatEtmeyeBasla(bina);
                            }
                        }
                        else if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Zemin")))
                        {
                            Birim b = mevcut_secili.GetComponentInParent<Birim>();
                            if (b != null)
                            {
                                b.Yuru(hit.point);
                            }
                        }
                    }

                    lm = LayerMask.GetMask("Bina");

                    if (lm == (lm | (1 << mevcut_secili.layer)))
                    {
                        // seçili olan þey bir bina

                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Zemin")))
                        {
                            Bina b = mevcut_secili.GetComponentInParent<Bina>();
                            if (b != null)
                            {
                                b.VarisSec(hit.point);
                            }
                        }
                    }
                }
            }
        }
        else if(mevcutMod == Mod.BinaÜret && eldekiBina != null)
        {
            // fare zemin üzerindeyse elimizdeki binanýn konumunu güncelle
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool binaYapilabilir = false;
            
            if (Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Zemin")))
            {
                eldekiBina.transform.position = hit.point;
                eldekiBina.SetActive(true);

                // binanýn rengini çakýþtýðý objelere göre güncelle
                if (Physics.CheckBox(eldekiBinaCollider.transform.position,
                    eldekiBinaCollider.bounds.size / 2,
                    Quaternion.identity, 
                    LayerMask.GetMask("Bina", "Kaynak", "Insaat")))
                {
                    eldekiBinaRenderer.material = mat_binaolmaz;
                }
                else
                {
                    eldekiBinaRenderer.material = mat_binaolur;
                    binaYapilabilir = true;
                }
                
            }
            else
            {
                eldekiBina.SetActive(false);
            }


            if (Input.GetMouseButtonDown(0))
            {
                // sað týklandýysa binayý koymaya çalýþ
                if(binaYapilabilir)
                {
                    mevcutMod = Mod.Normal;
                    eldekiBina.GetComponent<NavMeshObstacle>().enabled = true;
                    eldekiBinaRenderer.material = eldekiBinaOrijinalMat;
                    GameObject yerdekiBina = Instantiate(eldekiBina);
                    yerdekiBina.GetComponent<Bina>().InsaataBasla();

                    eldekiBina.SetActive(false);
                    eldekiBina = null;

                    if (mevcut_secili != null)
                    {
                        mevcut_secili.GetComponentInParent<Birim>().InsaatEtmeyeBasla(yerdekiBina.GetComponentInParent<Bina>());
                    }
                }

            }
            else if (Input.GetMouseButtonDown(1))
            {
                // sol týklandýysa normal moda geri dön
                mevcutMod = Mod.Normal;
                eldekiBina.SetActive(false);
                eldekiBina.GetComponent<NavMeshObstacle>().enabled = true;
                eldekiBinaRenderer.material = eldekiBinaOrijinalMat;
                eldekiBina = null;
            }
        }
        
        
    }

    Collider eldekiBinaCollider;
    MeshRenderer eldekiBinaRenderer;
    Material eldekiBinaOrijinalMat;
    public void Buton0()
    {
        mevcutMod = Mod.BinaÜret;
        eldekiBina = bina_katalogu[0];
        eldekiBina.GetComponent<NavMeshObstacle>().enabled = false;
        eldekiBinaCollider = eldekiBina.GetComponentInChildren<Collider>();
        eldekiBinaRenderer = eldekiBina.GetComponentInChildren<MeshRenderer>();
        eldekiBinaOrijinalMat = eldekiBinaRenderer.material;
    }
}
