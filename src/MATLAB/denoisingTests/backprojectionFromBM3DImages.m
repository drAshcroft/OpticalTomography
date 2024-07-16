

nProjections =length(origImages);

step=(length(origImages))/nProjections;

angleStep=360.0/(nProjections+1);

s=size(origImages{1});
sizeProjections = s(2);

reconImage = zeros([sizeProjections sizeProjections]);

clear filter;
B=(sizeProjections/-2):(sizeProjections/2-1);
filter(1:length(B)) =abs(B);
filter=filter./(length(filter)/2)^2*3.14;
han=hann(length(filter))';
%filter=filter.*han;

filter=fftshift(filter);

addition = zeros([sizeProjections sizeProjections]);
for I=1:nProjections
    sI=round( (I-1)*step+1);
  
    projection =doBM3DImageFilter( origImages{sI});
    filteredImagesBM3D{I}=projection;
    projection = projection(:,round(size(projection,1)/2))';
    
    Projections{I}=projection;
     
    fProjection =real(ifft(  fft(projection) .* filter ));
    
    
    for J=1:sizeProjections
       addition(:,J)= fProjection(J);
    end
    
    addition=imrotate(addition,angleStep*I,'nearest','crop');
    
    reconImage=reconImage + addition;
end

figure;imagesc(reconImage);colormap gray;
D=reconImage;

