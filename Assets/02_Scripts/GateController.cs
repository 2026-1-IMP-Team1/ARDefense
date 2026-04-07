using UnityEngine;

public class GateController : MonoBehaviour
{
    public GameObject gateVisual; // 모델링이나 이팩트 오브젝트를 따로 껐다켰다 하기 위한 변수입니다
    public EnemySpawner spawner;

    void Start()
    {
        CloseGate(); // 게임 시작 시에는 게이트를 닫아둡니다
    }

    public void OpenGate()
    {
        if (gateVisual != null) gateVisual.SetActive(true); // 게이트가 열리면 게이트의 모습이 화면에 보이게합니다
    }

    public void CloseGate()
    {
        if (gateVisual != null) gateVisual.SetActive(false);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) //G키를 누르면 게이트가 열립니다 (테스트용)
        {
            OpenGate();
        }
    }
}