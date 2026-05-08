using Microsoft.EntityFrameworkCore;
using QuanLyPhongKham.Models;
using QuanLyPhongKham.Repositories.Interfaces;
using QuanLyPhongKham.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyPhongKham.Services.Implementations
{
    public class ChuyenKhoaService : IChuyenKhoaService
    {
        private readonly IChuyenKhoaRepository _chuyenKhoaRepository;

        public ChuyenKhoaService(IChuyenKhoaRepository chuyenKhoaRepository)
        {
            _chuyenKhoaRepository = chuyenKhoaRepository;
        }
        public void Add(ChuyenKhoa entity)
        {
            _chuyenKhoaRepository.Add(entity);
        }



        public void Delete(int id)
        {
            ChuyenKhoa? chuyenKhoa = _chuyenKhoaRepository.GetById(id);
            if (chuyenKhoa == null) throw new Exception("Khong tim thay chuyen khoa");
            _chuyenKhoaRepository.Delete(chuyenKhoa);
        }

        public ICollection<ChuyenKhoa> GetAll()
        {
            return _chuyenKhoaRepository.GetAll();
        }

        public ChuyenKhoa? GetById(int id)
        {
            return _chuyenKhoaRepository.GetById(id);
        }

        public void Update(ChuyenKhoa entity)
        {
            _chuyenKhoaRepository.Update(entity);
        }
    }
}
