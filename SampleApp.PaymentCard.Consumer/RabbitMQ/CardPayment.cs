﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SampleApp.PaymentCard.Consumer.RabbitMQ
{
    public class CardPayment
    {
        public decimal Amount { get; set; }
        public string CardNumber { get; set; }
        public string Name { get; set; }
    }
}
