using UnityEngine;
using UnityEditor;

public class PongEditorTool : EditorWindow
{
    private const float PADDLE_HEIGHT = 60f;
    private const float PADDLE_WIDTH = 10f;
    private const float BALL_SIZE = 10f;
    private const float BALL_SPEED = 100f;

    private Rect leftPaddle;
    private Rect rightPaddle;
    private Rect ball;
    private Vector2 ballVelocity;

    private float lastUpdateTime;
    private int score = 0;
    private int speed_level = 0;
    private int high_score = 0;
    private int life = 5;

    private bool showText = false;
    private string text = "";
    private bool ballHitLeftPaddle = false;
    private bool ballHitRightPaddle = false;

    [MenuItem("Window/Pong Game")]
    public static void ShowWindow()
    {
        GetWindow<PongEditorTool>("Pong Game");
    }

    private void OnEnable()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        float windowWidth = position.width;
        float windowHeight = position.height;

        leftPaddle = new Rect(10, windowHeight / 2 - PADDLE_HEIGHT / 2, PADDLE_WIDTH, PADDLE_HEIGHT);
        rightPaddle = new Rect(windowWidth - 20, windowHeight / 2 - PADDLE_HEIGHT / 2, PADDLE_WIDTH, PADDLE_HEIGHT);
        ball = new Rect(windowWidth / 2 - BALL_SIZE / 2, windowHeight / 2 - BALL_SIZE / 2, BALL_SIZE, BALL_SIZE);

        ballVelocity = new Vector2(Random.value > 0.5f ? BALL_SPEED : -BALL_SPEED, Random.Range(-BALL_SPEED, BALL_SPEED));

        lastUpdateTime = (float)EditorApplication.timeSinceStartup;
        score = 0; 
        speed_level = 3;
        text = "";
        showText = false;
    }

    private void OnGUI()
    {
        if (Event.current.type == EventType.Repaint)
        {
            UpdateGame();
            Repaint();
        }

        // ゲームオブジェクトを描画
        EditorGUI.DrawRect(leftPaddle, Color.white);
        EditorGUI.DrawRect(rightPaddle, Color.white);
        EditorGUI.DrawRect(ball, Color.white);
        if (GUILayout.Button("Reset Game"))
        {
            life = 5;
            InitializeGame();
        }
        // スコアを表示
        GUILayout.Label("Score: " + score);
        GUILayout.Label("BallSpeedLevel:" + speed_level);
        GUILayout.Label("High_Score:" + high_score);
        GUILayout.Label("Life: " + life);

        // 中央に文字を描画
        if (showText && !string.IsNullOrEmpty(text))
        {
            GUIStyle style = new GUIStyle
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 24
            };

            Vector2 textSize = style.CalcSize(new GUIContent(text));
            float textX = (position.width - textSize.x) / 2;
            float textY = (position.height - textSize.y) / 3;
            style.normal.textColor = Color.white;
            GUI.Label(new Rect(textX, textY, textSize.x, textSize.y), text, style);
        }
    }

    private void UpdateGame()
    {
        float deltaTime = (float)EditorApplication.timeSinceStartup - lastUpdateTime;
        lastUpdateTime = (float)EditorApplication.timeSinceStartup;

        // パドルをマウス位置に追従させる
        Vector2 mousePosition = Event.current.mousePosition;
        leftPaddle.y = Mathf.Clamp(mousePosition.y - PADDLE_HEIGHT / 2, 0, position.height - PADDLE_HEIGHT);
        rightPaddle.y = Mathf.Clamp(mousePosition.y - PADDLE_HEIGHT / 2, 0, position.height - PADDLE_HEIGHT);

        // ボールの移動
        ball.x += ballVelocity.x * deltaTime*(speed_level);
        ball.y += ballVelocity.y * deltaTime*(speed_level);

        // ボールが上下に衝突したときの処理
        if (ball.y <= 0 || ball.y + BALL_SIZE >= position.height)
        {
            ballVelocity.y = -ballVelocity.y;
        }

        // ボールが左パドルに衝突したときの処理
        if (ball.Overlaps(leftPaddle))
        {
            if (!ballHitLeftPaddle)
            {
                ballVelocity.x = -ballVelocity.x;
                score++;
                speed_level = Random.Range(3, 10);
                ballHitLeftPaddle = true;
            }
        }
        else
        {
            ballHitLeftPaddle = false;
        }

        // ボールが右パドルに衝突したときの処理
        if (ball.Overlaps(rightPaddle))
        {
            if (!ballHitRightPaddle)
            {
                ballVelocity.x = -ballVelocity.x;
                score++;
                speed_level = Random.Range(3, 10);
                ballHitRightPaddle = true;
            }
        }
        else
        {
            ballHitRightPaddle = false;
        }
        if (life == 0)
        {
            speed_level = 0;
            text = "GAME OVER";
            showText = true;
        }
        // ボールが左右に外れたらリセット
        if (ball.x < 0 || ball.x > position.width)
        {
            life--;
            InitializeGame();
        }
    }
}