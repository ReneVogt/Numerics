namespace Revo.Numerics.Interpolation;

public interface IPolynomInterpolator
{
    double[] Interpolate(double[] x, double[] y);
}
