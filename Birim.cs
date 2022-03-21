using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum BirimSafha { Yok, KaynakEmriVar, KaynakTopluyor, KaynakBirakilacak, InsaatEmriVar, Insaat };

public class Birim : MonoBehaviour
{
    private NavMeshAgent agent;

    public string isim;
    public float can;
    public float can_kapasite;
    public float saldiri;
    public float savunma;
    public float buyu;
    public float buyu_kapasitesi;
    
    public int resim_indeksi;

    public float kaynak_kapasitesi;
    public float eldeki_kaynak;

    public bool kaynak_toplayabilir;

    public Kaynak atanan_kaynak;
    public int atanan_kaynak_toplama_noktasi;

    public GameObject atanan_nokta;
    public GameObject donus_noktasi;

    public int toplanan_kaynak_cinsi;

    public BirimSafha safha;

    private AnaOyun anaOyun;
    private Bina insaatEdilecekBina;

    void Start()
    {
        safha = BirimSafha.Yok;
        agent = GetComponent<NavMeshAgent>();
        anaOyun = GameObject.Find("AnaOyun").GetComponent<AnaOyun>();
    }

    public void InsaatEtmeyeBasla(Bina bina)
    {
        Yuru(bina.cikis_noktasi.transform.position);
        safha = BirimSafha.InsaatEmriVar;
        insaatEdilecekBina = bina;
    }

    void Update()
    {
        if (safha == BirimSafha.KaynakEmriVar)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        // kaynak toplama noktasýndayýz!
                        safha = BirimSafha.KaynakTopluyor;
                        atanan_kaynak.ToplamaNoktasiniAlmayaCalis(this, atanan_kaynak_toplama_noktasi);

                        // toplarken renk deðiþtir
                        MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
                        if (mr != null)
                        {
                            mr.material = anaOyun.mat_toplama;
                        }
                    }
                }
            }
        }

        if (safha == BirimSafha.KaynakTopluyor)
        {
            if (eldeki_kaynak < kaynak_kapasitesi)
            {
                atanan_kaynak.miktar -= Time.deltaTime * 2;
                if (atanan_kaynak.miktar <= 0)
                {
                    atanan_kaynak.miktar = 0;
                    safha = BirimSafha.Yok;
                }
                eldeki_kaynak += Time.deltaTime * 2;
            }
            else
            {
                eldeki_kaynak = kaynak_kapasitesi;

                // elimiz doldu, býrakacak yer bul
                if (anaOyun.KaynakBirakacakNoktaBul(this))
                {
                    safha = BirimSafha.KaynakBirakilacak;
                    atanan_kaynak.ToplamaNoktasiniBirak(atanan_kaynak_toplama_noktasi);
                    OtoYuru(donus_noktasi.transform.position);

                    // býrakacakken toplama renginden vazgeç
                    MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
                    if (mr != null)
                    {
                        mr.material = anaOyun.mat_normal;
                    }
                }
                else
                {
                    safha = BirimSafha.Yok;
                }
            }
        }

        if (safha == BirimSafha.KaynakBirakilacak)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        // kaynak býrakma noktasýndayýz!
                        anaOyun.KaynakBirak(this);

                        // yeniden kaynaða dön
                        OtoYuru(atanan_nokta.transform.position);
                        safha = BirimSafha.KaynakEmriVar;
                    }
                }
            }
        }

        if (safha == BirimSafha.InsaatEmriVar)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                    {
                        // bina insaa noktasindayiz
                        safha = BirimSafha.Insaat;

                    }
                }
            }
        }

        if(safha == BirimSafha.Insaat)
        {
            if(insaatEdilecekBina != null)
            {
                if(insaatEdilecekBina.InsaataKatkidaBulun(1 * Time.deltaTime))
                {
                    safha = BirimSafha.Yok;
                    insaatEdilecekBina = null;
                }
            }
        }
    }
    public void Yuru(Vector3 konum)
    {
        if(atanan_kaynak != null)
        {
            atanan_kaynak.ToplamaNoktasiniBirak(atanan_kaynak_toplama_noktasi);
            // býrakacakken toplama renginden vazgeç
            MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
            if (mr != null)
            {
                mr.material = anaOyun.mat_normal;
            }
        }
        safha = BirimSafha.Yok;
        agent.SetDestination(konum);
    }

    public void OtoYuru(Vector3 konum)
    {
        agent.SetDestination(konum);
    }

    public void AgentBelirle()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void BinayaGit(Bina b)
    {
        Yuru(b.cikis_noktasi.transform.position);
    }

    public void KaynagaGit(Kaynak k)
    {
        if (k.KaynakTopla(this))
        {
            // kaynak toplamaya baþlanabilir
            OtoYuru(atanan_nokta.transform.position);
            safha = BirimSafha.KaynakEmriVar;
        }
    }
}
