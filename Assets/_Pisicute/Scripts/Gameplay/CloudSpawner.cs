using System.Collections.Generic;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cloudPrefab;
    [SerializeField] private float spawnRate;
    [SerializeField] private float spawnWidth;
    [SerializeField] private float spawnHeight;
    [SerializeField] private float pathLength;
    [SerializeField] private float cloudSpeed;
    [SerializeField] private Vector2 cloudScale;

    private List<GameObject> clouds = new List<GameObject>();
    private float spawnTimer;

    private void Update()
    {
        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnRate)
        {
            spawnTimer = 0;
            SpawnCloud();
        }
        for (int i = clouds.Count - 1; i >= 0; i--)
        {
            clouds[i].transform.position += transform.forward * cloudSpeed * Time.deltaTime;
            if (clouds[i].transform.position.x < transform.position.x - pathLength)
            {
                Destroy(clouds[i]);
                clouds.RemoveAt(i);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + transform.right * spawnWidth + transform.up * spawnHeight, transform.position - transform.right * spawnWidth + transform.up * spawnHeight);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * pathLength);
    }

    private void SpawnCloud()
    {
        GameObject cloud = Instantiate(cloudPrefab, transform);
        cloud.transform.position = transform.position + transform.right * Random.Range(-spawnWidth, spawnWidth) + transform.up * Random.Range(spawnHeight - 1, spawnHeight + 1);
        cloud.transform.localScale = new Vector3(Random.Range(cloudScale.x, cloudScale.y), Random.Range(cloudScale.x, cloudScale.y), Random.Range(cloudScale.x, cloudScale.y));
        clouds.Add(cloud);
    }
}
