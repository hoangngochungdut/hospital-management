using QuanLyPhongKham.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        void Add(T entity);
        ICollection<T> GetAll();

        T? GetById(int id);

        void Update(T entity);

        void Delete(T entity);
        //T? GetByNguoiDungId(int nguoiDungId);
    }
}
