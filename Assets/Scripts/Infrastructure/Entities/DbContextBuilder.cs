using SqlCipher4Unity3D;
using UnityEngine;

namespace Infrastructure.Entities
{
    public class DbContextBuilder
    {
        public SQLiteConnection Connection;

        public DbContextBuilder()
        {
            Connection = new SQLiteConnection(Application.streamingAssetsPath + "/Database/TicTacToe.db", "Asdf+1234");
        }
        
    }
}

