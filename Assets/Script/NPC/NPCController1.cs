using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using JetBrains.Annotations;  // コルーチン用
using UnityEngine.Animations;

public class NPCController1 : MonoBehaviour
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



    private NavMeshAgent agent;// NavMesh上で移動するエージェント
    public Transform constrainedArea; // 制約領域のTransform
    public float walkSpeed = 3.0f;// NPCの移動速度
    public float minRoamingTime = 5.0f;// 散策状態の最小時間
    public float maxRoamingTime = 20.0f;// 散策状態の最大時間

    private Vector3 initialPosition;// プレイヤーオブジェクトへの参照
    private Quaternion initialRotation; // 初期の向きを保存
    private NPCState currentState;  // 現在のNPCの状態
    private ScenarioState scenarioState; // 現在のシナリオの状態
    private Vector3 currentDestination; // 現在の目的地
    private float StopTime;// 現在の待機時間
    private bool isScenario;// シナリオ中かどうかを示すフラグ

    public Transform playerTransform; // プレイヤーオブジェクトへの参照を保持する変数
    public float rotationSpeed = 5.0f;  // 向きを変更する速度
    public float approachDistance = 1.0f; // プレイヤーに接近する距離


    public Animator animator;
    public LookAtConstraint lookAtConstraint;


    // 歩行モーションのトリガー
    private const string WalkATrigger = "WalkA";
    private const string WalkBTrigger = "WalkB";
    private const string WalkCTrigger = "WalkC";


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        initialPosition = transform.position;// 初期の位置を保存
        initialRotation = transform.rotation; // 初期の向きを保存

        
            // 最初は待機状態
            currentState = NPCState.Idle;
            // 待機時間をランダムに設定して待機状態に遷移
            Invoke("ChangeStateTo_Roaming", Random.Range(minRoamingTime, maxRoamingTime));

        isScenario = false;




    }

    void Update()
    {

        // if (isScenario)
        // {
        //     // シナリオ中の場合、シナリオの状態に応じて更新
        //     scenarioState = ScenarioState.TrunAway;
        //     switch (scenarioState)
        //     {
        //         case ScenarioState.Wait:
        //             Debug.Log("Current State: Wait");
        //             UpdateWaitState();
        //             break;
        //         case ScenarioState.TrunAway:
        //             Debug.Log("Current State: TrunAway");
        //             UpdateTrunAwayState();
        //             break;

        //         case ScenarioState.Approach:
        //             Debug.Log("Current State: Approach");
        //             UpdateApproachState();
        //             break;
        //     }
        // }


        if(animator != null){
            AnimationCheck();
        }else{
            Debug.Log("animatorがありません");
        }

        if (!isScenario)
        {
            
             // シナリオ中でない場合、NPCの状態に応じて更新
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


    public void AnimationCheck(){
        if (agent.hasPath)
        {
            // NPCが目的地に向いているかチェック
            if (IsFacingDestination())
            {
                // 目的地に向いている場合、現在の角度に応じて歩行モーションを再生
                float angle = GetFacingAngle();
                PlayWalkAnimation(angle);
            }
            else
            {
                // 目的地に向いていない場合は歩行モーションを停止
                ResetWalkAnimation();
            }
        }
        else
        {
            // 目的地が設定されていない場合は歩行モーションを停止
            ResetWalkAnimation();
        }
    }
    
    // NPCが目的地に向いているかをチェックするメソッド
    bool IsFacingDestination()
    {
        Vector3 direction = (agent.steeringTarget - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.forward, direction);
        return dotProduct > 0.9f; // 任意の閾値を設定することも可能
    }

    // NPCが向いている角度を取得するメソッド
    float GetFacingAngle()
    {
        Vector3 direction = agent.steeringTarget - transform.position;
        //steeringTarget 移動している途中で現在向かっている目標地点を示すプロパティです。
        //から、NPCの位置を引いて、目標までの方向ベクトルを得る。
        direction.y = 0f; // （高さ成分の角度）y軸の回転を無視
        Quaternion rotation = Quaternion.LookRotation(direction);
        //与えられた方向ベクトルに対して、その方向を向くためのクォータニオン（四元数）を作成します。クォータニオンは、3次元空間内での回転を表現する数学的なオブジェクトです。
        //与えられた方向ベクトルに基づいて回転を計算し、その結果をQuaternionオブジェクトとして返します。
        return rotation.eulerAngles.y;
    }

    // 角度に応じて適切な歩行モーションを再生するメソッド
    void PlayWalkAnimation(float angle)
    {
        // 移動中でもオブジェクトの向きを固定
            agent.updateRotation = false;
        
        if (angle >= 315f || angle < 45f)//真横
        {
            animator.SetBool(WalkBTrigger, false);
            animator.SetBool(WalkCTrigger, false);
            animator.SetBool(WalkATrigger,true); // 歩行モーションA
        }
        else if (angle >= 45f && angle < 135f)//斜め
        {
            animator.SetBool(WalkATrigger, false);
            animator.SetBool(WalkCTrigger, false);
            animator.SetBool(WalkBTrigger,true); // 歩行モーションB
        }
        else if (angle >= 135f && angle < 225f)
            {
            animator.SetBool(WalkATrigger, false);
            animator.SetBool(WalkBTrigger, false);
            animator.SetBool(WalkCTrigger,true); // 歩行モーションC
        }
        else if (angle >= 225f && angle < 315f)
        {        
            // 右側の歩行モーションを反転して再生
            animator.SetBool(WalkCTrigger, false);
            animator.SetBool(WalkATrigger, false);
            animator.SetBool(WalkBTrigger, true); // 右側の歩行モーションBを再生
            // オブジェクトが180度回転していない場合のみ回転する
            if (transform.rotation.eulerAngles.y != 180f)
            {
                // アニメーションオブジェクトのrotationを180度回転させる
                transform.Rotate(0f, 180f, 0f);
            }
        }
    }




    void ResetWalkAnimation()
    {
        animator.SetBool(WalkATrigger, false);
        animator.SetBool(WalkBTrigger, false);
        animator.SetBool(WalkCTrigger, false);
    }

    // void UpdateWaitState()    // 待機状態の更新
    // {
    //   // 何もしない
    // }

    // void UpdateTrunAwayState()    // 背を向ける状態の更新

    // {

    //     if (playerTransform != null)
    //     {
    //         // プレイヤーの座標を取得
    //         Vector3 playerPosition = playerTransform.position;

    //         // プレイヤーの方向を向かずに背を向ける処理
    //         Vector3 directionToPlayer = transform.position - playerPosition;
    //         directionToPlayer.y = 0f;  // y軸の回転は考慮しない
    //         Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
    //         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

    //         // 背を向けるだけなので、移動は停止
    //         agent.SetDestination(transform.position);
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Player not found.");
    //     }

    // }



    // void UpdateApproachState()    // 接近状態の更新

    // {
    //     if (playerTransform != null)
    //     {
    //         // プレイヤーの座標を取得
    //         Vector3 playerPosition = playerTransform.position;

    //         // プレイヤーの方向を向く
    //         Vector3 directionToPlayer = playerPosition - transform.position;
    //         directionToPlayer.y = 0f;
    //         Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
    //         transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

    //         // プレイヤーに近づく処理を追加
    //         agent.SetDestination(playerPosition);
    //         // 一定距離以内にプレイヤーがいるかを判定
    //         float distanceToPlayer = Vector3.Distance(transform.position, playerPosition);
    //         if (distanceToPlayer < approachDistance)
    //         {
    //             // プレイヤーの目の前に来たら行いたい処理をここに追加
    //             isScenario = false;
    //             Debug.Log("Player is in front!");
    //         }
    //     }
    //     else
    //     {
    //         Debug.LogWarning("Player not found.");
    //     }
    // }







    void UpdateIdleState()    // 待機状態の更新
    {
        StopTime -= Time.deltaTime;
        if (StopTime <= 0.0f)
        {
            // 待機時間が終了したら散策状態か帰還状態にランダムで遷移
            if (Random.Range(0f, 1f) < 0.5f)
            {
                ChangeStateTo_Roaming();
            }
            else
            {
                ChangeStateTo_Returning();
            }
        }
    }


    void UpdateRoamingState()
    {
        // 目的地に到達したら待機状態に遷移
        if (IsDestinationReached())
        {
            ChangeStateTo_Idle();
        }
    }

    void UpdateReturningState()
    {
        // 初期位置に到達したら待機状態に遷移
        if (IsDestinationReached())
        {
            Debug.Log("Returning Complete");
            OnReturningComplete();
            ChangeStateTo_Idle();
        }
    }





    bool IsDestinationReached()
    {
        // 目的地に到達したかどうかを判定
        return !agent.pathPending && agent.remainingDistance < 0.1f;
    }






    void ChangeStateTo_Roaming()
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

    void ChangeStateTo_Idle()
    {
        // 待機状態に遷移
        currentState = NPCState.Idle;

        // 待機時間をランダムに設定
        StopTime = Random.Range(minRoamingTime, maxRoamingTime);
    }

    void ChangeStateTo_Returning()
    {
        // 帰還状態に遷移
        currentState = NPCState.Returning;

        // 初期位置に向かって移動を始める
        agent.SetDestination(initialPosition);
    }








    Vector3 GetRandomDestinationInConstrainedArea()    // 制約領域内でランダムな目的地を取得

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

    void OnReturningComplete()    // NPCが初期位置に戻った時に呼ばれる

    {
        // 待機状態に遷移
        ChangeStateTo_Idle();
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
