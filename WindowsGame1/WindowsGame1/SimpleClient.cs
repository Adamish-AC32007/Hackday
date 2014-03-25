using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Cassandra;

namespace WindowsGame1
{
     public class SimpleClient
    {
        private Cluster _cluster;

        public Cluster Cluster { get { return _cluster; } }

        private Session _session;

        public Session Session { get { return _session; } }
       


        public void CreateSchema() {
            _session.Execute("create keyspace IF NOT EXISTS playerData WITH replication ="+
           "{'class':'SimpleStrategy', 'replication_factor':1};");

            _session.Execute(
            "CREATE TABLE IF NOT EXISTS playerData.data (username varchar, accuracy float, totalshots int, meleekills int, totalkills int, highscore int, playtime uuid, PRIMARY KEY(username,playtime)) WITH CLUSTERING ORDER BY (playtime DESC);");
            
            _session.Execute(
            "CREATE TABLE IF NOT EXISTS playerData.friends (" +
            "username varchar PRIMARY KEY," +
            "friends set<varchar>" +
            ");");

            _session.Execute(
            "CREATE TABLE IF NOT EXISTS playerData.login (" +
            "username varchar PRIMARY KEY," +
            "password varchar" +
            ");");

         }


        public void LoadData(String username, float accuracy,int totalshots,int meleekills,int totalkills,int highscore, Guid playerTime) 
        {
             PreparedStatement statement = Session.Prepare("INSERT INTO playerData.data" +
             "(username, accuracy, totalshots, meleekills, totalkills, highscore, playtime) " +
             "VALUES (?, ?, ?, ?, ?, ?, ?);");

             BoundStatement boundStatement = new BoundStatement(statement);
             Session.Execute(boundStatement.Bind(
                   username,
                   accuracy,
                   totalshots,
                   meleekills,
                   totalkills,
                   highscore,
                   playerTime
                   ));
        }

         //not required anymore
         /*
        public bool searchUsername(string username)
        {
            bool found = false;
            PreparedStatement statement = Session.Prepare("Select * from playerData.data where username=?;");
            
            BoundStatement boundStatement = new BoundStatement(statement);
            RowSet rs = Session.Execute(boundStatement.Bind(username));

            if (rs != null)
            {
                foreach (Row row in rs.GetRows())
                {
                    row.GetValue<String>("username");
                }
                found = true;
            }

            return found;
        }
          * */

        public void Connect(String node)
        {
            _cluster = Cluster.Builder()
                .AddContactPoint(node).Build();
            Metadata metadata = _cluster.Metadata;

            Console.WriteLine("Connected to cluster: "
                + metadata.ClusterName.ToString());

            _session = _cluster.Connect();
        }

        public void Close()
        {
            _cluster.Shutdown();
        }

        public SimpleClient()
        {
            Connect("127.0.0.1");
            CreateSchema();
            return;
        }
    }
}
