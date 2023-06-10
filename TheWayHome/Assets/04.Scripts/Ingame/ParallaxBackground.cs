using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ParallaxBackground : MonoBehaviour
{
    //배경 스크롤 속도
    public float moveSpeed;

    private Material material;

    void Awake()
    {
        //렌더러의 머터리얼 값을 객체에 연결
        material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        // 1. 로비가 아닌 인게임에서만 배경 스크롤이 되게 한다. 2. 플레이어가 벽과 닿아있지 않을 때만 스크롤 되게 한다. 수직이동 시엔 스크롤 되면 이상하므로
        if(SceneManager.GetActiveScene().name != "Lobby" && !GameManager.Instance.player.GetComponent<PlayerController>().isWallTouch)
        {
            //무한 배경
            // 오른쪽으로 이동 -> 배경은 왼쪽으로 이동
            if (Input.GetKey(KeyCode.RightArrow))
            {
                float newOffSetX = material.mainTextureOffset.x + moveSpeed * Time.deltaTime;
                Vector3 newOffset = new Vector3(newOffSetX, 0, 0);
                material.mainTextureOffset = newOffset;
            }
            // 왼쪽으로 이동 -> 배경은 오른쪽으로 이동
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                float newOffSetX = material.mainTextureOffset.x + (moveSpeed * -1) * Time.deltaTime;
                Vector3 newOffset = new Vector3(newOffSetX, 0, 0);
                material.mainTextureOffset = newOffset;
            }
            // 입력 중지 -> 배경 이동 중지
            else if(Input.GetKeyUp(KeyCode.LeftArrow) ||  Input.GetKeyUp(KeyCode.RightArrow))
            {
                float newOffSetX = material.mainTextureOffset.x + (moveSpeed * 0) * Time.deltaTime;
                Vector3 newOffset = new Vector3(newOffSetX, 0, 0);
                material.mainTextureOffset = newOffset;
            }
        }      
    }
}
