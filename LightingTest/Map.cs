using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightingTest {
    class Map {
        private bool[,] _tiles;
        private int _width;
        private int _height;

        public int Width {
            get {
                return this._width;
            }
        }

        public int Height {
            get {
                return this._height;
            }
        }

        public Map(int width, int height) {
            this._tiles = new bool[width, height];
            this._width = width;
            this._height = height;
        }

        public bool this[int x, int y] {
            get {
                if(x < 0 || x >= this.Width || y < 0 || y >= this.Height) {
                    return false;
                }

                return this._tiles[x, y];
            }
            set {
                if(x < 0 || x >= this.Width || y < 0 || y >= this.Height) {
                    return;
                }

                this._tiles[x, y] = value;
            }
        }

        public static Map GetTestMap() {
            Map map = new Map(20, 20);

            map[5, 5] = true;
            map[6, 6] = true;
            map[6, 7] = true;

            return map;
        }
    }
}
