//$ Copyright 2015-22, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//
using UnityEngine;
using System.Collections.Generic;


namespace DungeonArchitect {
	/// <summary>
	/// A fast Gaussian blurring filter applied over a 2D data array
	/// </summary>
	public class BlurFilter : Filter {
		int radius;
		int w, h;

		public BlurFilter(int radius) {
			this.radius = radius;
		}

		public float[,] ApplyFilter (float[,] data)
		{
			var blurredData = new float[data.GetLength(0), data.GetLength(1)];
			gaussBlur_4(data, blurredData, radius);
			return blurredData;
		}

		// Algorithm from: http://blog.ivank.net/fastest-gaussian-blur.html
		void gaussBlur_4 (float[,] scl, float[,] tcl, int r) {
			h = scl.GetLength(0);
			w = scl.GetLength(1);
			var bxs = boxesForGauss(r, 3);
			boxBlur_4 (scl, tcl, (bxs[0]-1)/2);
			boxBlur_4 (tcl, scl, (bxs[1]-1)/2);
			boxBlur_4 (scl, tcl, (bxs[2]-1)/2);
		}
		void boxBlur_4 (float[,] scl, float[,] tcl, int r) {
			for(var y = 0; y < scl.GetLength(0); y++) {
				for(var x = 0; x < scl.GetLength(1); x++) {
					tcl[y, x] = scl[y, x];
				}
			}
			boxBlurH_4(tcl, scl, r);
			boxBlurT_4(scl, tcl, r);
		}

		float Get(float[,] data, int index) {
			var y = index / w;
			var x = index % w;
			return data[y, x];
		}

		void Set(float[,] data, int index, float value) {
			var y = index / w;
			var x = index % w;
			data[y, x] = value;
		}

		void boxBlurH_4 (float[,] scl, float[,] tcl, int r) {
			var iarr = 1.0f / (r+r+1);
			for(var i=0; i<h; i++) {
				int ti = i*w, li = ti, ri = ti+r;
				float fv = Get(scl, ti), lv = Get(scl, ti+w-1), val = (r+1)*fv;
				for(var j=0; j<r; j++) val += Get(scl, ti+j);
				for(var j=0  ; j<=r ; j++) { val += Get(scl, ri++) - fv       ;   Set(tcl, ti++, Round(val*iarr)); }
				for(var j=r+1; j<w-r; j++) { val += Get(scl, ri++) - Get(scl, li++);   Set(tcl, ti++, Round(val*iarr)); }
				for(var j=w-r; j<w  ; j++) { val += lv        - Get(scl, li++);   Set(tcl, ti++, Round(val*iarr)); }

				/*
				var ti = i*w, li = ti, ri = ti+r;
				var fv = scl[ti], lv = scl[ti+w-1], val = (r+1)*fv;
				for(var j=0; j<r; j++) val += scl[ti+j];
				for(var j=0  ; j<=r ; j++) { val += scl[ri++] - fv       ;   tcl[ti++] = Round(val*iarr); }
				for(var j=r+1; j<w-r; j++) { val += scl[ri++] - scl[li++];   tcl[ti++] = Round(val*iarr); }
				for(var j=w-r; j<w  ; j++) { val += lv        - scl[li++];   tcl[ti++] = Round(val*iarr); }
				*/
			}
		}
		void boxBlurT_4 (float[,] scl, float[,] tcl, int r) {
			var iarr = 1.0f / (r+r+1);
			for(var i=0; i<w; i++) {
				int ti = i, li = ti, ri = ti+r*w;
				float fv = Get(scl, ti), lv = Get(scl, ti+w*(h-1)), val = (r+1)*fv;
				for(var j=0; j<r; j++) val += Get(scl, ti+j*w);
				for(var j=0  ; j<=r ; j++) { val += Get(scl, ri) - fv     ;  Set(tcl, ti, Round(val*iarr));  ri+=w; ti+=w; }
				for(var j=r+1; j<h-r; j++) { val += Get(scl, ri) - Get(scl, li);  Set(tcl, ti, Round(val*iarr));  li+=w; ri+=w; ti+=w; }
				for(var j=h-r; j<h  ; j++) { val += lv      - Get(scl, li);  Set(tcl, ti, Round(val*iarr));  li+=w; ti+=w; }

				/*
				var ti = i, li = ti, ri = ti+r*w;
				var fv = scl[ti], lv = scl[ti+w*(h-1)], val = (r+1)*fv;
				for(var j=0; j<r; j++) val += scl[ti+j*w];
				for(var j=0  ; j<=r ; j++) { val += scl[ri] - fv     ;  tcl[ti] = Round(val*iarr);  ri+=w; ti+=w; }
				for(var j=r+1; j<h-r; j++) { val += scl[ri] - scl[li];  tcl[ti] = Round(val*iarr);  li+=w; ri+=w; ti+=w; }
				for(var j=h-r; j<h  ; j++) { val += lv      - scl[li];  tcl[ti] = Round(val*iarr);  li+=w; ti+=w; }
				*/
			}
		}
		
		int[] boxesForGauss(float sigma, int n)  // standard deviation, number of boxes
		{
			var wIdeal = Mathf.Sqrt((12*sigma*sigma/n)+1);  // Ideal averaging filter width 
			var wl = Mathf.FloorToInt(wIdeal);  if(wl%2==0) wl--;
			var wu = wl+2;
			
			var mIdeal = (12*sigma*sigma - n*wl*wl - 4*n*wl - 3*n)/(-4*wl - 4);
			var m = Mathf.Round(mIdeal);
			// var sigmaActual = Math.sqrt( (m*wl*wl + (n-m)*wu*wu - n)/12 );
			
			var sizes = new List<int>();  for(var i=0; i<n; i++) sizes.Add(i<m?wl:wu);
			return sizes.ToArray();
		}

		float Round(float a) {
			return a;
		}
	}
}
