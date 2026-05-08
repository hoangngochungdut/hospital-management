using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Services.Interfaces
{
    public interface IService<T>
    {
        void Add(T entity);
        ICollection<T> GetAll();

        T? GetById(int id);

        void Update(T entity);

        void Delete(int id);
    }
}
