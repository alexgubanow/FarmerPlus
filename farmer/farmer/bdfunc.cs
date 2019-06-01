using Npgsql;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace farmer
{
    public class bdfunc
    {
        public static void insert(string SQL)
        {
            try
            {
                using (var conn = new NpgsqlConnection("Server=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server") + ";Port=" + 
                    IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port") + ";User Id=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user") + 
                    ";Password=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "pass") + ";Database=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "database") + ";"))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;

                        // Insert some data
                        cmd.CommandText = SQL;
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Errr on insert!");
            }
        }
        public static void update(string SQL)
        {
            try
            {
                using (var conn = new NpgsqlConnection("Server=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server") + ";Port=" +
                    IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port") + ";User Id=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user") +
                    ";Password=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "pass") + ";Database=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "database") + ";"))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = SQL;
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Errr on update!");
            }
        }
        public static ArrayList maxid()
        {

            try
            {
                using (var conn = new NpgsqlConnection("Server=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server") + ";Port=" +
                    IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port") + ";User Id=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user") +
                    ";Password=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "pass") + ";Database=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "database") + ";"))
                {
                    conn.Open();
                    using (NpgsqlCommand comm = new NpgsqlCommand("SELECT MAX(ID) FROM contracts", conn))
                    {
                        using (var reader = comm.ExecuteReader())
                        {
                            ArrayList lstSelect = new ArrayList();
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    lstSelect.Add(reader[i]);
                                }
                            }
                            conn.Close();
                            return lstSelect;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Errr on maxid!");
                return null;
            }
        }
        public static void delete(string SQL)
        {
            try
            {
                using (var conn = new NpgsqlConnection("Server=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server") + ";Port=" +
                    IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port") + ";User Id=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user") +
                    ";Password=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "pass") + ";Database=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "database") + ";"))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = SQL;
                        cmd.ExecuteNonQuery();
                        conn.Close();
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Errr on delete!");
            }
        }
        public static List<contract> selcontr()
        {
            try
            {
                using (var conn = new NpgsqlConnection("Server=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server") + ";Port=" +
                    IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port") + ";User Id=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user") +
                    ";Password=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "pass") + ";Database=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "database") + ";"))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = "select * from contracts";
                        using (var reader = cmd.ExecuteReader())
                        {
                            List<contract> contracts = new List<contract>();
                            while (reader.Read())
                            {
                                contracts.Add(new contract
                                {
                                    id = Convert.ToInt32(reader[0]),
                                    contract_number = Convert.ToString(reader[1]),
                                    date_contract = Convert.ToString(reader[2]),
                                    lessor = Convert.ToString(reader[3]),
                                    cadastral_number = Convert.ToString(reader[4]),
                                    square = Convert.ToString(reader[5]),
                                    term_lease = Convert.ToString(reader[6]),
                                    registration_date = Convert.ToString(reader[7]),
                                    contract_status = Convert.ToString(reader[8]),
                                    unit_number = Convert.ToString(reader[9]),
                                    notes = Convert.ToString(reader[10]),
                                    type_contract = Convert.ToString(reader[11]),
                                    area_number = Convert.ToString(reader[12]),
                                    form_owner = Convert.ToString(reader[13]),
                                    form_use = Convert.ToString(reader[14]),
                                    form_unit = Convert.ToString(reader[15]),
                                    koatuu = Convert.ToString(reader[16]),
                                    leaser = Convert.ToString(reader[17]),
                                    contractortype = Convert.ToString(reader[18])
                                });
                            }
                            conn.Close();
                            return contracts;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Errr on select contract!");
                return null;
            }
        }
        public static ArrayList select(string SQL)
        {
            try
            {
                using (var conn = new NpgsqlConnection("Server=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server") + ";Port=" +
                    IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port") + ";User Id=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user") +
                    ";Password=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "pass") + ";Database=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "database") + ";"))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        cmd.CommandText = SQL;
                        using (var reader = cmd.ExecuteReader())
                        {
                            ArrayList lstSelect = new ArrayList();
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    lstSelect.Add(reader[i]);
                                }
                            }
                            conn.Close();
                            return lstSelect;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Errr on select!");
                return null;
            }
        }
        public static List<coordsformat> selcoords(List<string> id,string table)
        {
            try
            {
                using (var conn = new NpgsqlConnection("Server=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "server") + ";Port=" +
                    IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "port") + ";User Id=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "user") +
                    ";Password=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "pass") + ";Database=" + IniFile.Read(AppDomain.CurrentDomain.BaseDirectory + @"\settings.ini", "DB", "database") + ";"))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand())
                    {
                        cmd.Connection = conn;
                        List<coordsformat> lstSelect = new List<coordsformat>();
                        if (id.Count > 1)
                        {
                            cmd.CommandText = "SELECT* FROM " + table + " WHERE ID = '";
                            for (int l = 0; l < id.Count; l++)
                            {
                                if (l > 0)
                                {
                                    cmd.CommandText = cmd.CommandText + "' or ID='" + id[l];
                                }
                                else
                                {
                                    cmd.CommandText = cmd.CommandText + id[l];
                                }
                            }
                            cmd.CommandText = cmd.CommandText + "';";
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    for (int i = 0; i < reader.FieldCount / 4; i += 4)
                                    {
                                        lstSelect.Add(new coordsformat() { id = Convert.ToInt32(reader[i]), x = reader[i + 1].ToString(), y = reader[i + 2].ToString(), rank = Convert.ToInt32(reader[i + 3]) });
                                    }
                                }
                            }
                            conn.Close();
                        }
                        else
                        {
                            cmd.CommandText = "SELECT * FROM " + table + " WHERE ID='" + id[0] + "';";
                            using (var reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    for (int i = 0; i < reader.FieldCount / 4; i += 4)
                                    {
                                        lstSelect.Add(new coordsformat() { id = Convert.ToInt32(reader[i]), x = reader[i + 1].ToString(), y = reader[i + 2].ToString(), rank = Convert.ToInt32(reader[i + 3]) });
                                    }
                                }
                                conn.Close();
                            }
                        }
                        return lstSelect;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Errr on select coords!");
                return null;
            }
        }
        //public DataSet selecttab(string sql)
        //{
        //    try
        //    {
        //        this.getconnstr();
        //        this.OpenConn();

        //        DataSet lstSelect = new DataSet();
        //        NpgsqlDataAdapter da = new NpgsqlDataAdapter(sql, conn);
        //        lstSelect.Reset();
        //        da.Fill(lstSelect);

        //        this.CloseConn();

        //        return lstSelect;
        //    }
        //    catch (Exception)
        //    {
        //        MessageBox.Show("Errr on selecttab!");
        //        return null;
        //    }
        //}

    }
}
