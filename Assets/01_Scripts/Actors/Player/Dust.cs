
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.Mathf;

[ExecuteAlways]
public class Dust : MonoBehaviour
{
    public ParticleSystem ps;
    public Vector2[] positions;

    private ParticleSystem.Particle[] particles;

    public List<Vector2> PlotLine(Vector2 start, Vector2 end)
    {
        List<Vector2> list = new List<Vector2>();
        float dx = end.x - start.x;
        float dy = end.y - start.y;
        float length = Max(Abs(dx), Abs(dy));

        Vector2 delta = new Vector2(dx / length, dy / length);
        int i = 0;
        while (i < length)
        {
            list.Add(new Vector2(start.x, start.y));
            i++;
            start += delta;
        }

        return list;
    }

    public void UpdateLine()
    {
        List<Vector2> particlePositions = new List<Vector2>();

        for (int i = 0; i < positions.Length - 1; i++)
        {
            particlePositions.AddRange((PlotLine(positions[i], positions[i+1])));
        }
        
        ps.Clear();

        ParticleSystem.EmitParams param = new ParticleSystem.EmitParams();
        param.startSize = 1;
        param.startLifetime = 900;

        for (int i = 0; i < particlePositions.Count; i++)
        {
            param.position = particlePositions[i];
            ps.Emit(param, 1);
        }
    }
    
    void LateUpdate()
    {
        UpdateLine();

        // InitializeIfNeeded();
        //
        // // var partNums = ps.GetParticles(particles);
        // // for (int i = 0; i < partNums; i++)
        // // {
        // //     particles[i].position = Vector3Int.RoundToInt(particles[i].position);
        // // }
        // // ps.SetParticles(particles);
    }

    void InitializeIfNeeded()
    {
        if (ps == null)
            ps = GetComponent<ParticleSystem>();
        
        if (particles == null || particles.Length < ps.main.maxParticles) 
            particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }
    
    //Dust.Burst(BottomCenter, Calc.Up, 4);
    //Dust.Burst(Center + Vector2.UnitX * 2, Calc.UpLeft, 4); wall jump left
}


