using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    public bool isOrthographic;
    public bool devMode;

    [Header("Enemies")]
    public Wave[] waves;
    public Enemy enemyMelee;
    public Enemy enemyRanged;
    public Enemy enemyBoss;
    Enemy enemy;
    float graceTime, endLevelTime;

    LivingEntity playerEntity;
    Transform playerT;

    public Wave currentWave;
    int currentWaveNumber = 0;

    int enemiesRemainingToSpawn;
    int enemiesRemainingToSpawnInACamp;
    int enemiesRemainingAlive;
    float nextSpawnTime;
    float nextCampTime;

    [Header("Grace Time")]
    public float onHoldTime = 2f;
    public float onEndTime = 1f;

    MapGenerator map;

    float timeBetweenCampingChecks = 2f;
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 campPositionOld;
    bool isCamping;

    CameraFollow cameraFollower;

    bool isDisabled, startNextLevel;

    public Transform mapHolder;

    public event System.Action<int> OnNewWave;

    void Awake()
    {
        map = FindObjectOfType<MapGenerator>();
        cameraFollower = FindObjectOfType<CameraFollow>();
    }

    void Start()
    {

        playerEntity = FindObjectOfType<Player>();
        playerT = playerEntity.transform;

        nextCampCheckTime = Time.time;
        graceTime = Time.time + onHoldTime;
        campPositionOld = playerT.position;
        playerEntity.OnDeath += OnPlayerDeath;

        NextWave();
    }


    void Update()
    {
        if (startNextLevel)
        {
            if (Time.time > endLevelTime)
            {
                startNextLevel = false;

                // TO Do
                playerEntity.GetComponent<GunController>().EquipGunIndex(currentWaveNumber);
                //NextWave();

                
                
                cameraFollower.upgradesFlag = true;
                //cardDeck.PlaceCards();
                 
            }
        }

        if (!isDisabled)
        {
            // camp script
            /*
            if (Time.time > nextCampCheckTime)
            {
                nextCampCheckTime = timeBetweenCampingChecks + Time.time;

                isCamping = (Vector3.Distance(playerT.position, campPositionOld) < campThresholdDistance);
                campPositionOld = playerT.position;
            }*/

            if (enemiesRemainingToSpawnInACamp <= 0)
            {
                nextCampTime = Time.time + currentWave.timeBetweenCamps;
            }
            if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime && Time.time > graceTime && !currentWave.campMode)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

                StartCoroutine(SpawnEnemy());
            }
            else if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && enemiesRemainingToSpawnInACamp > 0 && Time.time > nextCampTime && Time.time > nextSpawnTime && Time.time > graceTime && currentWave.campMode)
            {
                enemiesRemainingToSpawn--;
                enemiesRemainingToSpawnInACamp--;

                StartCoroutine(SpawnEnemy());

                if (enemiesRemainingToSpawnInACamp == 1)
                {
                    enemiesRemainingToSpawnInACamp = currentWave.campSizes;
                    nextCampTime = Time.time + currentWave.timeBetweenCamps;
                }
                
            }
        }

        if (devMode)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StopCoroutine("SpawnEnemy");
                foreach (Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }
                NextWave();
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1f;
        float tileFlashSpeed = 4f;

        Transform spawnTile = map.GetRandomOpenTile();

        // camp script
        /*
        if (isCamping)
        {
            spawnTile = map.GetTileFromPosition(playerT.position);
        }*/
        Material tileMat = spawnTile.GetComponent<Renderer>().material;
        Color initialColor = tileMat.color;
        Color flashColor = Color.red;
        float spawnTimer = 0f;

        while (spawnTimer < spawnDelay)
        {
            tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }


        if (currentWave.waveType == Wave.enemyType.Melee)
        {
            enemy = enemyMelee;
        }

        else if (currentWave.waveType == Wave.enemyType.Ranged)
        {
            enemy = enemyRanged;
        }

        else if (currentWave.waveType == Wave.enemyType.Boss)
        {
            enemy = enemyBoss;
        }
        
        Enemy spawnedEnemy = Instantiate(enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemy.OnDeath += OnEnemyDeath;
        spawnedEnemy.SetCharacteristics(currentWave.moveSpeed, currentWave.enemyDamage, currentWave.enemyHealth, currentWave.skinColor);
    }

    void OnPlayerDeath()
    {
        isDisabled = true;
    }

    void OnEnemyDeath()
    {
        enemiesRemainingAlive--;
        if (enemiesRemainingAlive == 0)
        {
            startNextLevel = true;
            endLevelTime = Time.time + onEndTime;
            // TO DO


            //NextWave();
            //UpgradeLevel();
        }
    }

    void ResetPlayerPosition()
    {
        playerT.position = map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 2;
    }

    public void NextWave()
    {
        if (isOrthographic)
        {
            mapHolder.eulerAngles = new Vector3(0, 0, 0);
        }

        //FindObjectOfType<GameUI>().RefreshAmmo(FindObjectOfType<Player>().GetComponent<GunController>().equippedGun.projectilesRemainingInMag);
        if (currentWaveNumber > 0)
        {
            AudioManager.instance.PlaySound2D("Level Complete");
        }

        currentWaveNumber++;
        graceTime = Time.time + onHoldTime;

        if (currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];

            enemiesRemainingToSpawnInACamp = currentWave.campSizes;
            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;

            if (OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }
            ResetPlayerPosition();
        }

        if (FindObjectOfType<MapGenerator>().maps[currentWaveNumber - 1].mapSize.x > 16 || FindObjectOfType<MapGenerator>().maps[currentWaveNumber - 1].mapSize.y > 16)
        {
            Camera.main.GetComponent<CameraFollow>().SwitchCameraFollow(true);
        }

        if (isOrthographic)
        {
            mapHolder.eulerAngles = new Vector3(0, 45, 0);
        }

    }


    [System.Serializable]
    public class Wave
    {

        public enum enemyType
        {
            Melee,
            Ranged,
            Boss
        };

        public enemyType waveType;

        public bool infinite;
        public bool campMode;
        public int enemyCount;
        public int campSizes;
        public float timeBetweenSpawns;
        public float timeBetweenCamps;

        public float moveSpeed;
        public int enemyDamage;
        public float enemyHealth;
        public Color skinColor;
    }

}
