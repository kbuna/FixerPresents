using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using JetBrains.Annotations;  // コルーチン用

public class NPCController : MonoBehaviour
{
    private enum NPCState
    {
        Idle,       // 待機状態
        Roaming,    // 散策状態
        Returning   // 帰還状態
    }
    private enum ScenarioState
    {
        TrunAway, //背を向ける
        Wait,       // 待機状態
        Approach,    // 接近状態
    }

    private NavMeshAgent agent;
    public Transform constrainedArea; // 制約領域のTransform
    public float walkSpeed = 3.0f;
    public float minRoamingTime = 5.0f;
    public float maxRoamingTime = 20.0f;

    private Vector3 initialPosition;
    private Quaternion initialRotation; // 初期の向きを保存
    private NPCState currentState;
    private ScenarioState scenarioState;
    private Vector3 currentDestination;
    private float currentStopTime;
    private bool isScenario;
    public Transform playerTransform; // プレイヤーオブジェクトへの参照を保持する変数

    public float rotationSpeed = 5.0f; // メソッド内でのみ使用する場合、ここで宣言
    public float approachDistance = 1.0f;





    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        initialPosition = transform.position;
        initialRotation = transform.rotation; // 初期の向きを保存

        isScenario = true;




    }

    void Update()
    {

        if (isScenario)
        {
            scenarioState = ScenarioState.TrunAway;
            switch (scenarioState)
            {
                case ScenarioState.Wait:
                    Debug.Log("Current State: Wait");
                    UpdateWaitState();
                    break;
                case ScenarioState.TrunAway:
                    Debug.Log("Current State: TrunAway");
                    UpdateTrunAwayState();
                    break;

                case ScenarioState.Approach:
                    Debug.Log("Current State: Approach");
                    UpdateApproachState();
                    break;
            }
        }


        if (!isScenario)
        {

            // 最初は待機状態
            currentState = NPCState.Idle;
            // 待機時間をランダムに設定して待機状態に遷移
            Invoke("ChangeStateToRoaming", Random.Range(minRoamingTime, maxRoamingTime));
            switch (currentState)
            {
                case NPCState.Idle:
                    Debug.Log("Current State: Idle");
                    UpdateIdleState();
                    break;

                case NPCState.Roaming:
                    Debug.Log("Current State: Roaming");
                    UpdateRoamingState();
                    break;

                case NPCState.Returning:
                    Debug.Log("Current State: Returning");
                    UpdateReturningState();
                    break;
            }
        }
    }

    void UpdateWaitState()
    {

    }
    void UpdateTrunAwayState()
    {

        if (playerTransform != null)
        {
            // プレイヤーの座標を取得
            Vector3 playerPosition = playerTransform.position;

            // プレイヤーの方向を向かずに背を向ける処理
            Vector3 directionToPlayer = transform.position - playerPosition;
            directionToPlayer.y = 0f;  // y軸の回転は考慮しない
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // 背を向けるだけなので、移動は停止
            agent.SetDestination(transform.position);
        }
        else
        {
            Debug.LogWarning("Player not found.");
        }

    }





    void UpdateApproachState()
    {
        if (playerTransform != null)
        {
            // プレイヤーの座標を取得
            Vector3 playerPosition = playerTransform.position;

            // プレイヤーの方向を向く
            Vector3 directionToPlayer = playerPosition - transform.position;
            directionToPlayer.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

            // プレイヤーに近づく処理を追加
            agent.SetDestination(playerPosition);
            // 一定距離以内にプレイヤーがいるかを判定
            float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);
            if (distanceToPlayer < approachDistance)
            {
                // プレイヤーの目の前に来たら行いたい処理をここに追加
                isScenario = false;
                Debug.Log("Player is in front!");
            }
        }
        else
        {
            Debug.LogWarning("Player not found.");
        }
    }










    void UpdateIdleState()
    {
        // 待機時間の更新
        currentStopTime -= Time.deltaTime;
        if (currentStopTime <= 0.0f)
        {
            // 待機時間が終了したら散策状態か帰還状態にランダムで遷移
            if (Random.Range(0f, 1f) < 0.5f)
            {
                ChangeStateToRoaming();
            }
            else
            {
                ChangeStateToReturning();
            }
        }
    }


    void UpdateRoamingState()
    {
        // 目的地に到達したら待機状態に遷移
        if (IsDestinationReached())
        {
            ChangeStateToIdle();
        }
    }

    void UpdateReturningState()
    {
        // 初期位置に到達したら待機状態に遷移
        if (IsDestinationReached())
        {
            Debug.Log("Returning Complete");
            OnReturningComplete();
            ChangeStateToIdle();
        }
    }





    bool IsDestinationReached()
    {
        // 目的地に到達したかどうかを判定
        return !agent.pathPending && agent.remainingDistance < 0.1f;
    }






    void ChangeStateToRoaming()
    {
        // 散策状態に遷移
        currentState = NPCState.Roaming;

        // ランダムな目的地を制約領域内に設定
        currentDestination = GetRandomDestinationInConstrainedArea();

        // NavMeshAgentの速度を設定
        agent.speed = walkSpeed;

        // NavMeshAgentに目的地を設定して移動を始める
        agent.SetDestination(currentDestination);
    }

    void ChangeStateToIdle()
    {
        // 待機状態に遷移
        currentState = NPCState.Idle;

        // 待機時間をランダムに設定
        currentStopTime = Random.Range(minRoamingTime, maxRoamingTime);
    }

    void ChangeStateToReturning()
    {
        // 帰還状態に遷移
        currentState = NPCState.Returning;

        // 初期位置に向かって移動を始める
        agent.SetDestination(initialPosition);
    }








    Vector3 GetRandomDestinationInConstrainedArea()
    {
        Bounds bounds = constrainedArea.GetComponent<Collider>().bounds;
        Vector3 randomDestination;

        do
        {
            randomDestination = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                0f,
                Random.Range(bounds.min.z, bounds.max.z)
            );

        } while (!NavMesh.SamplePosition(randomDestination, out _, 1.0f, NavMesh.AllAreas));

        return randomDestination;
    }

    // NPCが初期位置に戻った時に呼ばれる
    void OnReturningComplete()
    {
        // 待機状態に遷移
        ChangeStateToIdle();
        // 向きを徐々に初期の向きに補完する
        StartCoroutine(SmoothRotation(initialRotation, 1.0f));
    }


    // 向きを徐々に補完するコルーチン
    IEnumerator SmoothRotation(Quaternion targetRotation, float duration)
    {
        float elapsed = 0.0f;
        Quaternion startRotation = transform.rotation;

        while (elapsed < duration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // 補完が終わったら確実に目標の向きにセットする
        transform.rotation = targetRotation;
    }




}
