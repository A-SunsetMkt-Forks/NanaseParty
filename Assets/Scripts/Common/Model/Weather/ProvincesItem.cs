using System.Collections.Generic;

namespace Common.Model.Weather
{
    public class ProvincesItem
    {
        /// <summary>
        /// ʡ��
        /// </summary>
        public string provinceName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<CitysItem> citys { get; set; }
    }
}
