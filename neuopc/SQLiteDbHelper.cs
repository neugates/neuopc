using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Data;
using Dapper;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace neuopc
{
    public class SQLiteDbHelper : IDisposable
    {
        const string INSERT_TABLE_ITEM_VALUE = "insert into {0} ({1}) values ({2})";
        const string DELETE_TABLE_WHERE = "delete from {0} where {1}";
        const string UPDATE_TABLE_EDITITEM = "update {0} set {1}";
        const string UPDATE_TABLE_EDITITEM_WHERE = "update {0} set {1} where {2}";
        const string Query_ITEM_TABLE_WHERE = "select {0} from {1} where {2}";

        private readonly SQLiteConnection _conn;

        public SQLiteDbHelper()
        {
            _conn = OpenDataConnection();
        }

        private static SQLiteConnection OpenDataConnection()
        {
            var conn = SQLiteBaseRepository.SimpleDbConnection();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            return conn;
        }

        public int Add<T>(T model, string autoPrimaryKey = "id")
        {
            var insertSql = GetInsertSql<T>(model, autoPrimaryKey);
            return _conn.Execute(insertSql);
        }

        public int Adds<T>(List<T> models, string autoPrimaryKey = "id")
        {
            var type = typeof(T);
            int resultN = 0;
            var transaction = _conn.BeginTransaction();
            try
            {
                models.ForEach(d =>
                {
                    var insertSql = GetInsertSql<T>(d);
                    resultN += _conn.Execute(insertSql);
                });
                transaction.Commit();
            }
            catch (Exception)
            {
                resultN = 0;
                transaction.Rollback();
            }
            return resultN;
        }

        public int Delete<T>(string where)
        {
            var type = typeof(T);
            string sqlStr = string.Format(DELETE_TABLE_WHERE, type.Name, where);
            return _conn.Execute(sqlStr);
        }

        public int Delete(string tableName, string where)
        {
            string sqlStr = string.Format(DELETE_TABLE_WHERE, tableName, where);
            return _conn.Execute(sqlStr);
        }

        public int Edit<T>(T model, string where, params string[] attrs)
        {
            var sqlStr = GetUpdateSql<T>(model, where, attrs);
            return _conn.Execute(sqlStr);
        }

        public T QeryByWhere<T>(string where, params string[] attrs)
        {
            Type type = typeof(T);
            string item = attrs.Length == 1 && attrs[0] == "*" ? "*" : string.Join(",", attrs);
            var sqlStr = string.Format(Query_ITEM_TABLE_WHERE, item, type.Name, where);
            return _conn.Query<T>(sqlStr).FirstOrDefault();
        }

        public List<T> QueryMultiByWhere<T>(string where)
        {
            Type type = typeof(T);
            var sqlStr = string.Format(Query_ITEM_TABLE_WHERE, "*", type.Name, where);
            return _conn.Query<T>(sqlStr).ToList();
        }

        private string GetInsertSql<T>(T model, string autoPrimaryKey = "id")
        {
            Type t = typeof(T);
            var propertyInfo = t.GetProperties();
            var proDic = propertyInfo
                .Where(
                    s => !s.Name.Equals(autoPrimaryKey, StringComparison.InvariantCultureIgnoreCase)
                )
                .Select(s => new { key = s.Name, value = GetValue<T>(s, model) })
                .ToDictionary(s => s.key, s => s.value);
            proDic = proDic.Where(s => s.Value != "''").ToDictionary(s => s.Key, s => s.Value);
            var items = string.Join(",", proDic.Keys);
            var values = string.Join(",", proDic.Values);
            return string.Format(INSERT_TABLE_ITEM_VALUE, t.Name, items, values);
        }

        private string GetValue<T>(PropertyInfo info, T model)
        {
            Type type = info.PropertyType;
            var tempStr = string.Empty;
            if (type == typeof(string))
            {
                tempStr = string.Format("'{0}'", info.GetValue(model));
                return tempStr;
            }
            if (type == typeof(DateTime))
            {
                tempStr = string.Format("'{0}'", ((DateTime)info.GetValue(model)).ToString("s"));
                return tempStr;
            }
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var types = type.GetGenericArguments();
                if (types[0] == typeof(DateTime))
                {
                    tempStr = string.Format(
                        "'{0}'",
                        ((DateTime)info.GetValue(model)).ToString("s")
                    );
                }
                tempStr = string.Format("'{0}'", info.GetValue(model));
                return tempStr;
            }
            tempStr = info.GetValue(model).ToString();
            return tempStr;
        }

        private string GetUpdateSql<T>(T model, string where, params string[] attrs)
        {
            Type t = typeof(T);
            var propertyInfo = t.GetProperties();
            var updateInfo = propertyInfo
                .Where(s => attrs.Contains(s.Name))
                .Select(s =>
                {
                    if (s.PropertyType == typeof(string))
                    {
                        return string.Format("{0}='{1}'", s.Name, s.GetValue(model));
                    }
                    if (s.PropertyType == typeof(DateTime))
                    {
                        return string.Format(
                            "{0}='{1}'",
                            s.Name,
                            ((DateTime)s.GetValue(model)).ToString("s")
                        );
                    }
                    if (
                        s.PropertyType.IsGenericType
                        && s.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                    )
                    {
                        Type[] types = s.PropertyType.GetGenericArguments();
                        if (types[0] == typeof(DateTime))
                        {
                            return string.Format(
                                "{0}='{1}'",
                                s.Name,
                                ((DateTime)s.GetValue(model)).ToString("s")
                            );
                        }
                        return string.Format("{0}={1}", s.Name, s.GetValue(model));
                    }
                    return string.Format("{0}={1}", s.Name, s.GetValue(model));
                })
                .ToArray();
            var setStr = string.Join(",", updateInfo);
            var sqlStr = string.Format(UPDATE_TABLE_EDITITEM_WHERE, t.Name, setStr, where);
            return sqlStr;
        }

        public void Dispose()
        {
            _conn.Close();
            _conn.Dispose();
        }
    }
}
