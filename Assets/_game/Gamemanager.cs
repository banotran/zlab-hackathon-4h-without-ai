
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable] public class CubeLayer
{
    public List<CubeMapItem> listCube= new List<CubeMapItem>();
    public int Preset;
    public ParticleSystem eff;
}
[System.Serializable] public class ShapeIndex
{
    public List<int> index;
}
[System.Serializable] public class PresetShape
{
   public List<ShapeIndex> Shape;
}
public class Gamemanager : Singleton<Gamemanager>
{
    [SerializeField] private int Col,Row;
    [SerializeField] private int layer;
    [SerializeField] private CubeMapItem g;
    [SerializeField] private GameObject g2;
    [SerializeField] private CubePick targetCube;
    [SerializeField] private Transform spawnCenter;
[ SerializeField] private float scaleDuration;
    [SerializeField] private List<CubeLayer> listG;

    [SerializeField] private Material materialBase;

    [SerializeField] private List<Color> listCo;

    [SerializeField] private Transform cubeSpawn;
    [SerializeField] private float spacing;

    [SerializeField] private CubeModule cube;
    [SerializeField] private float cubeScaleDuration;
    [SerializeField] private float moveDuration;
    [SerializeField] private float scaleDownDuration;

    [SerializeField] private List <PresetShape> preset;
    [SerializeField] private List<int> presetIndex;

    [SerializeField] private ParticleSystem eff;

    [SerializeField] private bool isInit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
       private int randomX;
       private int randomZ;
    private void RandomMissing()
    {
                randomX= Random.Range(0,Row);
        randomZ= Random.Range(0,Col);
    }
    void Start()
    {
        SpawnAsync().Forget();


    }
    	Ray ray;
	RaycastHit hit;
	
    int indexcall =0;
    private async UniTask DestroyLayer(int layer)
    {
        if (indexcall>0)
        {
            return;
        }
        indexcall++;//dirty
        
        int wait=0;
                                    foreach (CubeMapItem cube in listG[layer].listCube)
                            {
                                cube.transform.DOScale(0, scaleDownDuration).OnComplete(() =>
                                {
                                    cube.gameObject.SetActive(false);
                                //    Destroy(cube.gameObject,.04f);
                                wait--;
                                });
                                wait++;
                              //  await UniTask.Delay(20);
                            }
                            listG[layer].eff.Play();
                            
                            await UniTask.WaitUntil(()=>wait==0);

                            for(int i=0;i<listChoose.Count;i++)
                            {
                                Destroy(listChoose[i].gameObject);
                            }
                            listChoose.Clear();
                            indexcall=0;
                            //
                            if (layer -1 >=0)
                            {
                                  GenerateCube(listG[layer-1].Preset);
                            }
                          
    }
	void Update()
	{
		if (Input.GetMouseButtonDown(0)&&isInit)
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if(Physics.Raycast(ray, out hit))
		{
            if ( hit.collider.GetComponentInParent<CubeModule>())
            {
            
				print(hit.collider.name);
                Debug.Log($"[Cube] Cube my love");
                cube = hit.collider.GetComponentInParent<CubeModule>();
                cube.transform.DOScale(1.1f,cubeScaleDuration).SetEase(Ease.OutQuad);
            }
            else
                {
                    if(cube)
                    cube .transform.DOScale(1,cubeScaleDuration);
                }
             
            if (hit.collider.GetComponent<CubeMapItem>())
                {   
                        //           Debug.LogError($"cube : {hit.collider.name}, check cubemap = {hit.collider.GetComponent<CubeMapItem>()==null}");
                    CubeMapItem o = hit.collider.GetComponent<CubeMapItem>();
                    if (o.render.gameObject.activeSelf==false)
                    {
                    if (cube)
                    {
                        if (cube.GetComponent<CubePick>().Preset != o.Preset)
                            {
                               if (o.Preset!=4||cube.GetComponent<CubePick>().Preset!=0)
                                {
                                     Debug.LogError($"cube.GetComponent<CubePick>().Preset = {cube.GetComponent<CubePick>().Preset }");
                                Debug.LogError($"o.Preset == {o.Preset}");
                                return;
                                }
                            }
                        int indexLayer = o .layer;
                     
                        foreach(Transform tran in cube.GetComponent<CubePick>().ListPick)
                            {
                                 foreach (CubeMapItem item in listG[indexLayer].listCube )
                                {
                                   if (item.render.gameObject.activeSelf==false&&item.transform.childCount==1)
                                    {
                                           tran.transform.parent = item.transform;
                                           continue;
                                    }
                                }
                                tran.transform.DOScale(1,moveDuration);
                                tran.transform.DOLocalMove(Vector3.zero, moveDuration).SetEase(Ease.OutQuad).OnComplete(() =>
                                {
                                    DestroyLayer(o.layer).Forget();
                                });
                                tran.transform.DOLocalRotate(Vector3.zero,moveDuration).SetEase(Ease.OutQuad);
                                tran.GetComponentInChildren<Renderer>().material = o.render.material;
                            }
                            Debug.LogError("done");
 
                    }
                    else
                    {
                                      //Debug.LogError("?");

                    }
                              //    Debug.LogError(hit.collider.name);

                    }

                }


		}
            else
            {
                    if (cube)
                {
                    cube .transform.DOScale(1,cubeScaleDuration);
                }
            }
        }
	}
    [SerializeField] private List<GameObject> listChoose= new List<GameObject>();
    private void GenerateCube(int index)
    {
     

        GameObject target =  SpawnTargetParent();
        listChoose.Add(target);
        GameObject target2 =  SpawnTargetParent();
        target2.transform.localPosition +=Vector3.forward *4.5f;

        GameObject target3 =  SpawnTargetParent();
                target3.transform.localPosition +=Vector3.forward *9;

                    listChoose.Add(target2);
                        listChoose.Add(target3);
                          List<GameObject> shuffledList = listChoose.OrderBy( x => Random.value ).ToList( );
        if ( index >preset.Count-1)
        {
                GameObject a = Instantiate(g2.gameObject, target.transform);
        }
        else
        {
            SpawnTarget(shuffledList[0],Random.Range(0,preset.Count));
            SpawnTarget(shuffledList[1],index);
            SpawnTarget(shuffledList[2],Random.Range(0,preset.Count));
        }

    }
    private GameObject SpawnTargetParent()
    {
                GameObject target =  Instantiate(targetCube.gameObject,cubeSpawn.transform);
        target.gameObject.AddComponent<CubeModule>();
        target.gameObject.SetActive(true);
         return target;
    }

    private void SpawnTarget(GameObject target,int index)
    {
        target.GetComponent<CubePick>().Preset = index;
                        int rowStart = Row/2;
                int colStart = Col/2;
                Vector3 StartPos = Vector3.zero - Vector3.right*rowStart -Vector3.forward*colStart;
                float     firstx  =StartPos.x;
                float firstz=StartPos.z;
                for (int i =0;i<Row;i++)
            {
            for (int j=0;j<Col;j++)
                {
                            // Debug.LogError($"i :{i}, j: {j}");
                        if (preset[index].Shape[i].index[j]==1)
                        {

                                GameObject a = Instantiate(g2.gameObject, target.transform);
                                target.GetComponent<CubePick>().ListPick.Add(a.transform);
                                      a.name=$"CubeTarget[{i},{j}]";
                                a.transform.localPosition = StartPos;
                                a.gameObject.SetActive(true);
                             
                                //a.transform.localScale = Vector3.zero;
                               //a.render.gameObject.SetActive(false);
                        } 
                           StartPos += Vector3.forward*spacing;
                }
                    StartPos.z= firstz;
                    StartPos +=Vector3.right*spacing;
            }
    }
    private async UniTask SpawnAsync()
    {

              int rowStart = Row/2;
        int colStart = Col/2;
        int colorIndex=0;




        //
        ParticleSystem _eff = Instantiate(this.eff,this.transform);
        listG.Add (new CubeLayer()
        {
            eff = _eff
        });

        //RandomMissing();
        
        int presetIndex =0;
                  if (presetIndex>preset.Count-1)
            {
                RandomMissing();
            }
            else
            {
                randomX = -1;
                randomZ =-1;
              //  presetIndex++;
            }


        //
        Vector3 StartPos = Vector3.zero - Vector3.right*rowStart -Vector3.forward*colStart;
                  _eff.transform.localPosition = new Vector3(0, StartPos.y,0);
                  _eff.startColor = listCo[0];
        float     firstx  =StartPos.x;
        float firstz=StartPos.z;
        for ( int aa =0;aa<layer;aa++)
        {
            for (int i =0;i<Row;i++)
            {
            for (int j=0;j<Col;j++)
            {
                      CubeMapItem a = Instantiate(g, spawnCenter);
            a.transform.localPosition = StartPos;
            a.gameObject.SetActive(true);
            StartPos += Vector3.forward*spacing;
            a.transform.localScale = Vector3.zero;
            a.transform.DOScale(1,scaleDuration);
            a.name+=$"[{i},{j},{aa}]";
            a.layer= aa;
            a.Preset = presetIndex;

          Material mat=  a.render.material ;
          if (randomX!=-1&&randomZ!=-1)
                    {
                                    if (i==randomX&&j==randomZ)
                    {
                        a.render.gameObject.SetActive(false);
                    }
                    }
                    else
                    {
                       // Debug.LogError($"i :{i}, j: {j},index:{presetIndex}");
                        if (preset[presetIndex].Shape[i].index[j]==1)
                        {
                               a.render.gameObject.SetActive(false);
                        }
                    }
          mat.color = listCo[colorIndex];
           listG[aa].listCube.Add(a);
           listG[aa].Preset =presetIndex;
               await UniTask.Delay(10);
            }
            
            StartPos.z= firstz;
            StartPos +=Vector3.right*spacing;

            await UniTask.Delay(20);
            }
             presetIndex++;
             this.presetIndex.Add(presetIndex);
            if (presetIndex>preset.Count-1)
            {
                RandomMissing();
            }
            else
            {
                randomX = -1;
                randomZ =-1;
               
            }
                  //listG.Add (new CubeLayer());
               colorIndex++;
            StartPos.x = firstx;
            StartPos.z=firstz; 
            StartPos+=Vector3.up;
         //   ParticleSystem _eff;
                ParticleSystem __eff = Instantiate(this.eff,this.transform);
        listG.Add (new CubeLayer()
        {
            eff = __eff
        });
        __eff.transform.localPosition = new Vector3(0, StartPos.y,0);
        __eff.startColor =   listCo[colorIndex];
            
        }
            GenerateCube(0);
            isInit=true;
    }
    
}
