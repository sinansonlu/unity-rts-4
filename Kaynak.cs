using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kaynak : MonoBehaviour
{
    public float miktar;
    public int kaynak_cinsi;
    public GameObject[] toplama_noktalari;

    private bool[] toplama_noktasi_kullanim;

    // Start is called before the first frame update
    void Start()
    {
        toplama_noktasi_kullanim = new bool[toplama_noktalari.Length];
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToplamaNoktasiniBirak(int i)
    {
        toplama_noktasi_kullanim[i] = false;
    }

    public void ToplamaNoktasiniAlmayaCalis(Birim b, int i)
    {
        if(!toplama_noktasi_kullanim[i])
        {
            toplama_noktasi_kullanim[i] = true;
        }
        else
        {
            if (KaynakTopla(b))
            {
                b.safha = BirimSafha.KaynakEmriVar;
            }
        }
    }

    public bool KaynakTopla(Birim b)
    {
        for(int i = 0; i < toplama_noktasi_kullanim.Length; i++)
        {
            if(!toplama_noktasi_kullanim[i])
            {
                b.atanan_nokta = toplama_noktalari[i];
                
                // elde baþka kaynak varken toplamaya sýfýrdan baþla
                if(b.toplanan_kaynak_cinsi != kaynak_cinsi)
                {
                    b.eldeki_kaynak = 0;
                }

                b.toplanan_kaynak_cinsi = kaynak_cinsi;
                b.atanan_kaynak = this;
                b.atanan_kaynak_toplama_noktasi = i;
                return true;
            }
        }

        return false;
    }
}
