using Jt808TerminalEmulator.Model.Dtos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jt808TerminalEmulator.Core
{
    /// <summary>
    /// 经纬度插值
    /// </summary>
    public class LocationInterpolation
    {
        ILogger logger;
        public LocationInterpolation(ILogger<LocationInterpolation> logger)
        {
            this.logger = logger;
        }
        readonly long radius = 6371393; // 地球的平均半径，以km为单位
        readonly double pi = Math.PI;

        /// <summary>
        /// 辅助功能将度数转换为弧度
        /// </summary>
        /// <param name="deg"></param>
        /// <returns></returns>
        public double DegToRad(double deg)
        {
            return (deg * pi / 180);
        }

        /// <summary>
        /// 辅助功能将弧度转换为度
        /// </summary>
        /// <param name="rad"></param>
        /// <returns></returns>
        public double RadToDeg(double rad)
        {
            return (rad * 180 / pi);
        }

        /// <summary>
        /// 计算两点之间的（初始）方位角，以度为单位
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public double CalculateBearing(LocationDto startPoint, LocationDto endPoint)
        {
            var lat1 = DegToRad(startPoint.Latitude);
            var lat2 = DegToRad(endPoint.Latitude);
            var deltaLon = DegToRad(endPoint.Logintude - startPoint.Logintude);

            var y = Math.Sin(deltaLon) * Math.Cos(lat2);
            var x = Math.Cos(lat1) * Math.Sin(lat2) - Math.Sin(lat1) * Math.Cos(lat2) * Math.Cos(deltaLon);
            var bearing = Math.Atan2(y, x);

            // 由于atan2返回的值介于-180和+180之间，因此我们需要将其转换为0-360度
            return (RadToDeg(bearing) + 360) % 360;
        }

        /// <summary>
        /// 在给定的初始方位上，从已经走了给定距离（以km为单位）的给定点计算出目的地点（在到达目的地之前，轴承可能会有所不同）
        /// </summary>
        /// <param name="point"></param>
        /// <param name="bearing"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        public LocationDto CalculateDestinationLocation(LocationDto point, double bearing, double distance)
        {

            distance /= radius; // 转换为弧度的角距离
            bearing = DegToRad(bearing); // 将方位角转换为弧度

            var lat1 = DegToRad(point.Latitude);
            var lon1 = DegToRad(point.Logintude);

            var lat2 = Math.Asin(Math.Sin(lat1) * Math.Cos(distance) + Math.Cos(lat1) * Math.Sin(distance) * Math.Cos(bearing));
            var lon2 = lon1 + Math.Atan2(Math.Sin(bearing) * Math.Sin(distance) * Math.Cos(lat1), Math.Cos(distance) - Math.Sin(lat1) * Math.Sin(lat2));
            lon2 = (lon2 + 3 * pi) % (2 * pi) - pi; // 标准化为-180 <-> +180度

            return new LocationDto { Latitude = RadToDeg(lat2), Logintude = RadToDeg(lon2) };
        }

        /// <summary>
        /// 计算两点之间的距离（以km为单位）
        /// </summary>
        /// <param name="startPoint"></param>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        public double CalculateDistanceBetweenLocations(LocationDto startPoint, LocationDto endPoint)
        {

            var lat1 = DegToRad(startPoint.Latitude);
            var lon1 = DegToRad(startPoint.Logintude);

            var lat2 = DegToRad(endPoint.Latitude);
            var lon2 = DegToRad(endPoint.Logintude);

            var deltaLat = lat2 - lat1;
            var deltaLon = lon2 - lon1;

            var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) + Math.Cos(lat1) * Math.Cos(lat2) * Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return (radius * c);
        }

        /// <summary>
        /// 给定行进距离（以km为单位），以计算出两点之间距离起点的位置
        /// </summary>
        /// <param name="startLocation"></param>
        /// <param name="endLocation"></param>
        /// <param name="distanceTravelled">离起点的距离</param>
        /// <returns></returns>
        public LocationDto IntermediaryLocation(LocationDto startLocation, LocationDto endLocation, double distanceTravelled)
        {
            var bearing = CalculateBearing(startLocation, endLocation);
            return CalculateDestinationLocation(startLocation, bearing, distanceTravelled);
        }

        /// <summary>
        /// 根据两个点以、速度、持续时间获取每秒所产生的点
        /// </summary>
        /// <param name="startLocation">起点</param>
        /// <param name="endLocation">终端</param>
        /// <param name="speed">速度</param>
        /// <param name="duration">持续时间，单位秒</param>
        /// <returns></returns>
        public List<LocationDto> IntermediaryLocation(LocationDto startLocation, LocationDto endLocation, double speed, long duration)
        {
            var locations = new List<LocationDto>();

            for (int i = 0; i < duration; i++)
            {
                double bearing = CalculateBearing(startLocation, endLocation);
                double distanceInKm = speed / 1000;
                var intermediaryLocation = CalculateDestinationLocation(startLocation, bearing, distanceInKm);

                locations.Add(intermediaryLocation);

                // 将中介位置设置为新的起始位置
                startLocation = intermediaryLocation;
            }
            return locations;
        }

        /// <summary>
        /// 重新插值
        /// </summary>
        /// <param name="dto">线路</param>
        /// <param name="speed">速度</param>
        /// <param name="interval">间隔时间</param>
        /// <param name="alldistance">线路总长度，单位m</param>
        /// <returns></returns>
        public List<LocationDto> Reinterpolation(LineDto dto, double speed, int interval, out double alldistance)
        {
            var intervalDistance = speed / 3.6 * interval;
            alldistance = 0d;
            var locations = new List<LocationDto>
            {
                dto.Locations.First()
            };
            var nextDistance = intervalDistance;
            var startLocation = locations.Last();
            for (int i = 1; i < dto.Locations.Count - 1; i++)
            {
                var endLocation = dto.Locations[i];
                alldistance += CalculateDistanceBetweenLocations(dto.Locations[i - 1], endLocation);
                while (true)
                {
                    var distance = CalculateDistanceBetweenLocations(startLocation, endLocation);
                    if (distance < nextDistance)
                    {
                        nextDistance -= distance;
                        startLocation = endLocation;
                        break;
                    }
                    nextDistance = intervalDistance;
                    startLocation = IntermediaryLocation(startLocation, endLocation, intervalDistance);
                    locations.Add(startLocation);
                }
            }
            return locations;
        }

        /// <summary>
        /// 获取指定速度下行驶时长后线中的点
        /// </summary>
        /// <param name="localtions">线路中的点</param>
        /// <param name="startLocation">当前点</param>
        /// <param name="speed">速度，单位km/h</param>
        /// <param name="interval">间隔时间，单位s</param>
        /// <param name="nextIndex">下一个关键点的索引</param>
        /// <returns></returns>
        public LocationDto GetNextLation(List<LocationDto> localtions, LocationDto startLocation, double speed, int interval, ref int nextIndex)
        {
            if (nextIndex >= localtions.Count) return null;
            var intervalDistance = speed / 3.6 * interval;
            var nextDistance = intervalDistance;
            for (; nextIndex < localtions.Count;)
            {
                var distance = CalculateDistanceBetweenLocations(startLocation, localtions[nextIndex]);
                if (distance < nextDistance)
                {
                    nextDistance -= distance;
                    startLocation = localtions[nextIndex];
                    nextIndex++;
                    continue;
                }
                var endLocation = IntermediaryLocation(startLocation, localtions[nextIndex], nextDistance);
                distance = CalculateDistanceBetweenLocations(endLocation, localtions[nextIndex]);
                logger.LogInformation($"当前索引：{nextIndex}，差值{distance}起点[{startLocation.Logintude},{startLocation.Latitude}]终点,[{endLocation.Logintude},{endLocation.Latitude}],第{localtions[nextIndex].Order}个关键点[{localtions[nextIndex].Logintude},{localtions[nextIndex].Latitude}]");
                return endLocation;
            }
            return default;
        }
    }
}
