using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ParallaxBackground : MonoBehaviour
{
    //��� ��ũ�� �ӵ�
    public float moveSpeed;

    private Material material;

    void Awake()
    {
        //�������� ���͸��� ���� ��ü�� ����
        material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        // 1. �κ� �ƴ� �ΰ��ӿ����� ��� ��ũ���� �ǰ� �Ѵ�. 2. �÷��̾ ���� ������� ���� ���� ��ũ�� �ǰ� �Ѵ�. �����̵� �ÿ� ��ũ�� �Ǹ� �̻��ϹǷ�
        if(SceneManager.GetActiveScene().name != "Lobby" && !GameManager.Instance.player.GetComponent<PlayerController>().isWallTouch)
        {
            //���� ���
            // ���������� �̵� -> ����� �������� �̵�
            if (Input.GetKey(KeyCode.RightArrow))
            {
                float newOffSetX = material.mainTextureOffset.x + moveSpeed * Time.deltaTime;
                Vector3 newOffset = new Vector3(newOffSetX, 0, 0);
                material.mainTextureOffset = newOffset;
            }
            // �������� �̵� -> ����� ���������� �̵�
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                float newOffSetX = material.mainTextureOffset.x + (moveSpeed * -1) * Time.deltaTime;
                Vector3 newOffset = new Vector3(newOffSetX, 0, 0);
                material.mainTextureOffset = newOffset;
            }
            // �Է� ���� -> ��� �̵� ����
            else if(Input.GetKeyUp(KeyCode.LeftArrow) ||  Input.GetKeyUp(KeyCode.RightArrow))
            {
                float newOffSetX = material.mainTextureOffset.x + (moveSpeed * 0) * Time.deltaTime;
                Vector3 newOffset = new Vector3(newOffSetX, 0, 0);
                material.mainTextureOffset = newOffset;
            }
        }      
    }
}
