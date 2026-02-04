using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

using System.Data;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;


public class DatabaseTest : SerializedMonoBehaviour
{
    [SerializeField]
    [Multiline(20)]
    private string display;

    [SerializeField]    
    private string sqlString;

    [SerializeField]    
    private Account foundAccount;

    [SerializeField]
    private List<WeaponItem> weapons;

    [Button]
    private void Go()
    {
        weapons = new List<WeaponItem>();

        //string queryString = "select * from weapon limit 1";        
        string connectionString = "Data Source=127.0.0.1;Initial Catalog=mechgamedatabase;User ID=PManhatton;Password=8H211EucsUjC$IZyCg5YU9@y";

        MySqlConnection connection = new MySqlConnection(connectionString);

        connection.Open();
        var query = connection.Query<WeaponItem>(sqlString);

        weapons.AddRange(query);

        connection.Close();
    }

    private class WeaponItem
    {
        public string name;
        public string description;
        public float damage;
    }


}
