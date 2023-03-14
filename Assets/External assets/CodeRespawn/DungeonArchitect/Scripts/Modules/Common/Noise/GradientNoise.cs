//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using System.Collections.Generic;
using UnityEngine;

namespace DungeonArchitect.Utils.Noise
{

    public interface INoiseTable<T>
    {
        void Init(int InSize, System.Random random);
        float GetNoise(float u, float v);
        NoiseTableCell<T> GetCell(float x, float y);
    }

    public interface INoisePolicy<T>
    {
        float Sample(float x, float y, INoiseTable<T> NoiseTable);
        T GetRandom(System.Random random);
    }

    public struct NoiseTableCell<T>
    {
        public T N00;
        public T N10;
        public T N01;
        public T N11;
    };

    public abstract class NoiseTable<T> : INoiseTable<T>
    {
        private int size;
        private List<T> data = new List<T>();
        protected INoisePolicy<T> noisePolicy;

        protected abstract INoisePolicy<T> CreateNoisePolicy();

	    public void Init(int size, System.Random random) {
		    this.size = size;
            noisePolicy = CreateNoisePolicy();

            int NumElements = size * size;
            data = new List<T>(new T[NumElements]);

		    int LastIdx = size - 1;
			for (int y = 0; y < size; y++) {
				for (int x = 0; x < size; x++) {
					if (x == LastIdx || y == LastIdx) {
						int ix = x % LastIdx;
						int iy = y % LastIdx;
						data[IDX(x, y)] = data[IDX(ix, iy)];
					}
					else {
                        data[IDX(x, y)] = noisePolicy.GetRandom(random);
					}
				}
			}
	    }

        public NoiseTableCell<T> GetCell(float x, float y)
        {
            int x0 = Mathf.FloorToInt(x) % size;
            int y0 = Mathf.FloorToInt(y) % size;
            int x1 = (x0 + 1) % size;
            int y1 = (y0 + 1) % size;

            var cell = new NoiseTableCell<T>();
            cell.N00 = data[IDX(x0, y0)];
            cell.N10 = data[IDX(x1, y0)];
            cell.N01 = data[IDX(x0, y1)];
            cell.N11 = data[IDX(x1, y1)];
            return cell;
        }

        public float GetNoise(float u, float v)
        {
            float x = u * (size - 1);
            float y = v * (size - 1);

            return noisePolicy.Sample(x, y, this);
        }

        public float GetNoiseFBM(Vector2 p, int octaves)
        {
            p /= size;
            float noise = 0;
            float amp = 1; // 0.457345f;
            for (int i = 0; i < octaves; i++)
            {
                noise += amp * GetNoise(p.x, p.y);
                p *= 1.986576f;
                amp *= 0.5f;
            }
            noise = 0.5f + noise * 0.5f;
            return Mathf.Clamp01(noise);
        }

        T GetTableData(int x, int y)
        {
            return data[IDX(x, y)];
        }

        int GetSize()
        {
            return size;
        }

        int IDX(int x, int y)
        {
            return y * size + x;
        }
    }

    public class GradientNoisePolicy : INoisePolicy<Vector2>
    {
        public float Sample(float x, float y, INoiseTable<Vector2> NoiseTable)
        {
            var Cell = NoiseTable.GetCell(x, y);

            float fx = x % 1;
            float fy = y % 1;
            var P = new Vector2(fx, fy);

            return Mathf.Lerp(
                Mathf.Lerp(
                    Vector2.Dot(Cell.N00, P - new Vector2(0, 0)),
                    Vector2.Dot(Cell.N10, P - new Vector2(1, 0)),
                    fx),
                Mathf.Lerp(
                    Vector2.Dot(Cell.N01, P - new Vector2(0, 1)),
                    Vector2.Dot(Cell.N11, P - new Vector2(1, 1)),
                    fx),
                fy);
        }

        public Vector2 GetRandom(System.Random random)
        {
            var angle = random.NextFloat() * Mathf.PI * 2.0f;
            return new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle));
        }
    }

    public class GradientNoiseTable : NoiseTable<Vector2>
    {
        protected override INoisePolicy<Vector2> CreateNoisePolicy()
        {
            return new GradientNoisePolicy();
        }
    }
}
