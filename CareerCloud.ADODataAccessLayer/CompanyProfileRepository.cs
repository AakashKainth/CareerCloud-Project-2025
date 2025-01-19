using CareerCloud.DataAccessLayer;
using CareerCloud.Pocos;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Transactions;

namespace CareerCloud.ADODataAccessLayer
{
    public class CompanyProfileRepository : IDataRepository<CompanyProfilePoco>
    {
        private readonly string _connStr;

        public CompanyProfileRepository()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();

            _connStr = configuration.GetConnectionString("DataConnection");
            if (string.IsNullOrEmpty(_connStr))
            {
                throw new InvalidOperationException("Connection string 'DataConnection' is not found or empty.");
            }
        }

        public void Add(params CompanyProfilePoco[] items)
        {
            TransactionManager.ImplicitDistributedTransactions = true; // Enable implicit distributed transactions

            if (items == null || items.Length == 0)
            {
                throw new ArgumentException("No items to add.");
            }

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (CompanyProfilePoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "CompanyProfilePoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand(@"INSERT INTO [dbo].[Company_Profiles]
                                                              ([Id], [Registration_Date], [Company_Website], [Contact_Phone], [Contact_Name], [Company_Logo])
                                                              VALUES (@Id, @Registration_Date, @Company_Website, @Contact_Phone, @Contact_Name, @Company_Logo)", conn, transaction);

                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.Parameters.AddWithValue("@Registration_Date", item.RegistrationDate);
                            cmd.Parameters.AddWithValue("@Company_Website", item.CompanyWebsite);
                            cmd.Parameters.AddWithValue("@Contact_Phone", item.ContactPhone);
                            cmd.Parameters.AddWithValue("@Contact_Name", item.ContactName);
                            cmd.Parameters.AddWithValue("@Company_Logo", item.CompanyLogo);

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void CallStoredProc(string name, params Tuple<string, string>[] parameters)
        {
            throw new NotImplementedException();
        }

        public IList<CompanyProfilePoco> GetAll(params Expression<Func<CompanyProfilePoco, object>>[] navigationProperties)
        {
            List<CompanyProfilePoco> pocos = new List<CompanyProfilePoco>();

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Company_Profiles]", conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CompanyProfilePoco poco = new CompanyProfilePoco
                        {
                            Id = reader.GetGuid(0),
                            RegistrationDate = reader.GetDateTime(1),
                            CompanyWebsite = reader.IsDBNull(2) ? null : reader.GetString(2),
                            ContactPhone = reader.GetString(3),
                            ContactName = reader.IsDBNull(4) ? null : reader.GetString(4),
                            CompanyLogo = reader.IsDBNull(5) ? null : (byte[])reader[5],
                            TimeStamp = (byte[])reader[6]
                        };
                        pocos.Add(poco);
                    }
                }
            }

            return pocos;
        }

        public IList<CompanyProfilePoco> GetList(Expression<Func<CompanyProfilePoco, bool>> where, params Expression<Func<CompanyProfilePoco, object>>[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public CompanyProfilePoco GetSingle(Expression<Func<CompanyProfilePoco, bool>> where, params Expression<Func<CompanyProfilePoco, object>>[] navigationProperties)
        {
            IList<CompanyProfilePoco> pocos = GetAll();
            return pocos.AsQueryable().Where(where).FirstOrDefault();
        }

        public void Remove(params CompanyProfilePoco[] items)
        {
            TransactionManager.ImplicitDistributedTransactions = true; // Enable implicit distributed transactions

            if (items == null || items.Length == 0)
            {
                throw new ArgumentException("No items to remove.");
            }

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (CompanyProfilePoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "CompanyProfilePoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[Company_Profiles] WHERE Id = @Id", conn, transaction);
                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Update(params CompanyProfilePoco[] items)
        {
            TransactionManager.ImplicitDistributedTransactions = true; // Enable implicit distributed transactions

            if (items == null || items.Length == 0)
            {
                throw new ArgumentException("No items to update.");
            }

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (CompanyProfilePoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "CompanyProfilePoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand(@"UPDATE [dbo].[Company_Profiles]
                                                              SET [Registration_Date] = @Registration_Date, [Company_Website] = @Company_Website, [Contact_Phone] = @Contact_Phone, [Contact_Name] = @Contact_Name, [Company_Logo] = @Company_Logo
                                                              WHERE Id = @Id", conn, transaction);

                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.Parameters.AddWithValue("@Registration_Date", item.RegistrationDate);
                            cmd.Parameters.AddWithValue("@Company_Website", item.CompanyWebsite);
                            cmd.Parameters.AddWithValue("@Contact_Phone", item.ContactPhone);
                            cmd.Parameters.AddWithValue("@Contact_Name", item.ContactName);
                            cmd.Parameters.AddWithValue("@Company_Logo", item.CompanyLogo);

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}




