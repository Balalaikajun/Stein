using Application.DTOs.Group;
using System;

namespace Application.DTOs.Metrics;

public record ContingentHistogramRequest(
    DateOnly Date,
    bool? IsFullTime = null);