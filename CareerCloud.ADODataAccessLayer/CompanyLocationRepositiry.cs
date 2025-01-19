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
    public class CompanyLocationRepository : IDataRepository<CompanyLocationPoco>
    {
        private readonly string _connStr;

        public CompanyLocationRepository()
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

        public void Add(params CompanyLocationPoco[] items)
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
                        foreach (CompanyLocationPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "CompanyLocationPoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand(@"INSERT INTO [dbo].[Company_Locations]
                                                              ([Id], [Company], [Country_Code], [State_Province_Code], [Street_Address], [City_Town], [Zip_Postal_Code])
                                                              VALUES (@Id, @Company, @Country_Code, @State_Province_Code, @Street_Address, @City_Town, @Zip_Postal_Code)", conn, transaction);

                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.Parameters.AddWithValue("@Company", item.Company);
                            cmd.Parameters.AddWithValue("@Country_Code", item.CountryCode);
                            cmd.Parameters.AddWithValue("@State_Province_Code", item.Province);
                            cmd.Parameters.AddWithValue("@Street_Address", item.Street);
                            cmd.Parameters.AddWithValue("@City_Town", item.City);
                            cmd.Parameters.AddWithValue("@Zip_Postal_Code", item.PostalCode);

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

        public IList<CompanyLocationPoco> GetAll(params Expression<Func<CompanyLocationPoco, object>>[] navigationProperties)
        {
            List<CompanyLocationPoco> pocos = new List<CompanyLocationPoco>();

            using (SqlConnection conn = new SqlConnection(_connStr))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM [dbo].[Company_Locations]", conn);
                conn.Open();

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        CompanyLocationPoco poco = new CompanyLocationPoco
                        {
                            Id = reader.GetGuid(0),
                            Company = reader.GetGuid(1),
                            CountryCode = reader.GetString(2),
                            Province = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Street = reader.IsDBNull(4) ? null : reader.GetString(4),
                            City = reader.IsDBNull(5) ? null : reader.GetString(5),
                            PostalCode = reader.IsDBNull(6) ? null : reader.GetString(6),
                            TimeStamp = (byte[])reader[7]
                        };
                        pocos.Add(poco);
                    }
                }
            }

            return pocos;
        }

        public IList<CompanyLocationPoco> GetList(Expression<Func<CompanyLocationPoco, bool>> where, params Expression<Func<CompanyLocationPoco, object>>[] navigationProperties)
        {
            throw new NotImplementedException();
        }

        public CompanyLocationPoco GetSingle(Expression<Func<CompanyLocationPoco, bool>> where, params Expression<Func<CompanyLocationPoco, object>>[] navigationProperties)
        {
            IList<CompanyLocationPoco> pocos = GetAll();
            return pocos.AsQueryable().Where(where).FirstOrDefault();
        }

        public void Remove(params CompanyLocationPoco[] items)
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
                        foreach (CompanyLocationPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "CompanyLocationPoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand("DELETE FROM [dbo].[Company_Locations] WHERE Id = @Id", conn, transaction);
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

        public void Update(params CompanyLocationPoco[] items)
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
                        foreach (CompanyLocationPoco item in items)
                        {
                            if (item == null)
                            {
                                throw new ArgumentNullException(nameof(item), "CompanyLocationPoco item is null.");
                            }

                            SqlCommand cmd = new SqlCommand(@"UPDATE [dbo].[Company_Locations]
                                                              SET [Company] = @Company, [Country_Code] = @Country_Code, [State_Province_Code] = @State_Province_Code, [Street_Address] = @Street_Address, [City_Town] = @City_Town, [Zip_Postal_Code] = @Zip_Postal_Code
                                                              WHERE Id = @Id", conn, transaction);

                            cmd.Parameters.AddWithValue("@Id", item.Id);
                            cmd.Parameters.AddWithValue("@Company", item.Company);
                            cmd.Parameters.AddWithValue("@Country_Code", item.CountryCode);
                            cmd.Parameters.AddWithValue("@State_Province_Code", item.Province);
                            cmd.Parameters.AddWithValue("@Street_Address", item.Street);
                            cmd.Parameters.AddWithValue("@City_Town", item.City);
                            cmd.Parameters.AddWithValue("@Zip_Postal_Code", item.PostalCode);

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



