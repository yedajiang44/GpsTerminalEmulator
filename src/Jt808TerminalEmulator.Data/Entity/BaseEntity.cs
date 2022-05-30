using System;
using System.Collections.Generic;
using System.Text;

namespace Jt808TerminalEmulator.Data.Entity
{
    /// <summary>
    /// 基础实体
    /// </summary>
    public class BaseEntity : IEntity<string>
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTimeOffset CreateDateTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 创建人Id
        /// </summary>
        public string CreateUserId { get; set; }
    }

    /// <summary>
    /// 基础实体扩展
    /// </summary>
    public static class BaseEntityExtention
    {
        /// <summary>
        /// 默认值初始化
        /// </summary>
        /// <param name="entity"></param>
        public static void Init(this BaseEntity entity)
        {
            entity.Id = Guid.NewGuid().ToString("N");
            entity.CreateDateTime = DateTimeOffset.UtcNow;
        }

        /// <summary>
        /// 用户初始化
        /// </summary>
        public static void CreateBy(this BaseEntity entity, string userId)
        {
            entity.Init();
            entity.CreateUserId = userId;
        }
    }
}
