using System;
using System.Collections.Generic;
using System.Text;

namespace FileProcessing.Infrastructure.Jwt.Dto
{
    public interface IRefreshTokenDto
    {
        public  string RFToken { get; set; }
        public DateTime RFCreatedToken { get; set; }
        public DateTime RFExpireToken { get; set; }
    }
    public class RefreshTokenDto : IRefreshTokenDto
    {
        public required string RFToken { get; set; }
        public required DateTime RFCreatedToken { get; set; }
        public required DateTime RFExpireToken { get; set; }
    }
}
