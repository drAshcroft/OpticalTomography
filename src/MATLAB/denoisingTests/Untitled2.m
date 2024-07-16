h0=double(imread('fluor0.tif'));

addH0=2*(h0+3/8).^.5;

[image psf]=deconvblind(addH0,zeros([150 150])+1,5);
figure;
imagesc(image);
colormap gray;
title('anscombe');

figure;
imagesc(psf);
colormap gray;
title('psf');

figure;
flip = (image./2).^2-3/8;
imagesc(flip);
colormap gray;
title('inverse anscombe');

[image0 psf]=deconvblind(h0,zeros([150 150])+1,5);
figure;
imagesc(image0);
colormap gray;
title('deconv. original');

flip2=.25*image.^2+1/4*(3/2)^.5*image.^-1-11/8*image.^-2+5/8*(3/2)^.5*image.^-3-1/8;
figure;
imagesc(flip2);
colormap gray;
title('inverse 2');
